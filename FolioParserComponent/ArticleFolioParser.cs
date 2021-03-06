﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace FolioParserComponent
{
    internal class ArticleFolioParser : IArticleFolioParser
    {
        public async Task<Article> ParseAsync(IStorageFile articleFolioFile)
        {
            Article article = new Article();

            Stream stream = await articleFolioFile.OpenStreamForReadAsync();

            XmlReader reader = XmlReader.Create(stream);

            // skips the information we actually need to start with
            reader.ReadToFollowing("contentStacks");

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    switch (reader.Name)
                    {
                        case "contentStack":
                            article.Id = reader.GetAttribute("id");
                            break;
                        case "title":
                            reader.Read();
                            article.Title = reader.Value;
                            break;
                        case "description":
                            reader.Read();
                            article.Description = reader.Value;
                            break;
                        case "author":
                            reader.Read();
                            article.Author = reader.Value;
                            break;
                        case "kicker":
                            reader.Read();
                            article.Kicker = reader.Value;
                            break;
                        case "assets":
                            article.Pages = parsePages(reader);
                            break;
                        case "overlays":
                            article = parseOverlays(reader, article);
                            break;
                        default:
                            break;
                    }
                }
            }
            stream.Dispose();
            return article;
        }


        public IList<Page> parsePages(XmlReader reader)
        {
            IList<Page> pages = new List<Page>();
            while (reader.Read())
            {
                if (reader.Name == "asset")
                {
                    if (reader.GetAttribute("landscape") == "false") // create a portrait page 
                    {
                        pages.Add(parsePortraitPage(reader));
                    }
                    else if (reader.GetAttribute("landscape") == "true") // add landscape assets to existing page or create new one if it doesn't exist.
                    {
                        bool found = false;
                        Page newPage = parseLandscapePage(reader);
                        // find the existing page in the List of pages and adds the landscape data to it
                        // or if the landscape page is an extra sub page inject it before the next page
                        foreach (Page p in pages)
                        {
                            if (p.Id == newPage.Id)
                            {
                                // if the id's are perfect matches or newPage doesn't have a sub id
                                if(p.SubId == newPage.SubId || newPage.SubId == null)
                                {
                                    found = true;
                                    p.LandscapeContentUrl = newPage.LandscapeContentUrl;
                                    p.LandscapeContentWidth = newPage.LandscapeContentWidth;
                                    p.LandscapeContentHeight = newPage.LandscapeContentHeight;

                                    p.LandscapeThumbUrl = newPage.LandscapeThumbUrl;
                                    p.LandscapeThumbWidth = newPage.LandscapeThumbWidth;
                                    p.LandscapeThumbHeight = newPage.LandscapeThumbHeight;

                                    p.LandscapeScrubberUrl = newPage.LandscapeScrubberUrl;
                                    p.LandscapeScrubberWidth = newPage.LandscapeScrubberWidth;
                                    p.LandscapeScrubberHeight = newPage.LandscapeScrubberHeight;
                                    break;
                                }
                            }
                            // if there is more landscape subpages than portrait subpages for a given page
                            else if (Convert.ToInt32(p.Id) > Convert.ToInt32(newPage.Id))
                            {
                                found = true;
                                pages.Insert(pages.IndexOf(p), newPage);
                                break;
                            }
                        }
                        if (!found)  // a landscape page that does not already have a portrait page in the list
                        {
                            pages.Add(newPage);
                        }
                    }
                }
                else if (reader.Name == "assets")
                {
                    break;
                }
            }
            return pages;
        }

        public Page parsePortraitPage(XmlReader reader)
        {
            Page page = new Page();
            while (reader.Read())
            {
                // indicates next page
                if (reader.Name == "asset")
                {
                    break;
                }
                if (reader.Name == "assetRendition")
                {
                    var role = reader.GetAttribute("role");
                    switch (role)
                    {
                        case "content":
                            page.PortraitContentUrl = reader.GetAttribute("source");
                            page.PortraitContentWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.PortraitContentHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        case "thumbnail":
                            page.PortraitThumbUrl = reader.GetAttribute("source");
                            page.PortraitThumbWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.PortraitThumbHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        case "scrubber":
                            page.PortraitScrubberUrl = reader.GetAttribute("source");
                            page.PortraitScrubberWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.PortraitScrubberHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        default:
                            break;
                    }
                }
            }

            // parses the integers off the end of the Id from the xml
            
            Regex idRegex = new Regex("\\d+");
            var ids = idRegex.Matches(page.PortraitContentUrl);
            //main page number
            page.Id = ids[0].ToString();
            //sub page number is second element in the matches array if it is a subpage
            if (ids.Count > 1)
            {
                page.SubId = ids[1].ToString();
            }
            return page;
        }

        public Page parseLandscapePage(XmlReader reader)
        {
            Page page = new Page();
            while (reader.Read())
            {
                // indicates next page
                if (reader.Name == "asset")
                {
                    break;
                }
                if (reader.Name == "assetRendition")
                {
                    var role = reader.GetAttribute("role");
                    switch (role)
                    {
                        case "content":
                            page.LandscapeContentUrl = reader.GetAttribute("source");
                            page.LandscapeContentWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.LandscapeContentHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        case "thumbnail":
                            page.LandscapeThumbUrl = reader.GetAttribute("source");
                            page.LandscapeThumbWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.LandscapeThumbHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        case "scrubber":
                            page.LandscapeScrubberUrl = reader.GetAttribute("source");
                            page.LandscapeScrubberWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.LandscapeScrubberHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        default:
                            break;
                    }
                }
            }
            // parses the integers off the end of the Id from the xml
            Regex idRegex = new Regex("\\d+");
            var ids = idRegex.Matches(page.LandscapeContentUrl);
            //main page number
            page.Id = ids[0].ToString();
            //sub page number is second element in the matches array if it is a subpage
            if (ids.Count > 1)
            {
                page.SubId = ids[1].ToString();
            }

            return page;
        }

        // searches the pages list for the proper page to add overlay to
        // creates an overlay for each orientation if there are more than one
        public Article parseOverlays(XmlReader reader, Article article)
        {
            while (reader.ReadToFollowing("overlay"))
            {
                
                double portraitX = -1;
                double portraitY = -1;
                double landscapeX = -1;
                double landscapeY = -1;
                double portraitWidth = -1;
                double portraitHeight = -1;
                double landscapeWidth = -1;
                double landscapeHeight = -1;

                Tuple<int, double> portraitTuple = new Tuple<int, double>(-1, -1);
                Tuple<int, double> landscapeTuple = new Tuple<int, double>(-1, -1);

                string type = reader.GetAttribute("type");
                string id = reader.GetAttribute("id");

                while (reader.Read())
                {
                    // indicates the end of the overlay
                    if (reader.Name == "data")
                    {
                        break;
                    }
                    else if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        //assigns the attributes for portrait orientation if it exists
                        if (reader.Name == "portraitBounds")
                        {
                            if (reader.ReadToFollowing("rectangle"))
                            {
                                portraitX = Convert.ToDouble(reader.GetAttribute("x"));
                                portraitY = Convert.ToDouble(reader.GetAttribute("y"));
                                portraitWidth = Convert.ToDouble(reader.GetAttribute("width"));
                                portraitHeight = Convert.ToDouble(reader.GetAttribute("height"));
                                portraitTuple = calculatePortraitPage(portraitY, article.Pages);
                            }
                        }
                        //assigns the attributes for landscape orientation if it exists
                        else if (reader.Name == "landscapeBounds")
                        {
                            if (reader.ReadToFollowing("rectangle"))
                            {
                                landscapeX = Convert.ToDouble(reader.GetAttribute("x"));
                                landscapeY = Convert.ToDouble(reader.GetAttribute("y"));
                                landscapeWidth = Convert.ToDouble(reader.GetAttribute("width"));
                                landscapeHeight = Convert.ToDouble(reader.GetAttribute("height"));
                                landscapeTuple = calculateLandscapePage(landscapeY, article.Pages);
                            }
                        }
                    }
                }

                switch (type)
                {
                    case "audio":
                        AudioOverlay portraitAO = new AudioOverlay();
                        AudioOverlay landscapeAO = new AudioOverlay();

                        // getting the overlay assets for each oreintation if there are any
                        do
                        {
                            if (reader.Name == "audioUrl")
                            {
                                break;
                            }
                            else if (reader.Name == "overlayAsset")
                            {
                                if (reader.GetAttribute("landscape") == "false")
                                {
                                    portraitAO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                                else if (reader.GetAttribute("landscape") == "true")
                                {
                                    landscapeAO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                            }
                        } while (reader.Read());

                        AudioOverlay ao = parseAudioOverlay(reader);

                        if (portraitTuple.Item1 != -1)  // there exists a portrait orientation for this overlay
                        {
                            portraitAO.Id = id + "_P";
                            portraitAO.Orientation = "portrait";
                            portraitAO.X = portraitX;
                            portraitAO.Y = portraitTuple.Item2;
                            portraitAO.Width = portraitWidth;
                            portraitAO.Height = portraitHeight;
                            portraitAO.AudioUrl = ao.AudioUrl;
                            portraitAO.AutoStart = ao.AutoStart;
                            portraitAO.AutoStartDelay = ao.AutoStartDelay;
                            article.Pages[portraitTuple.Item1].AudioOverlays.Add(portraitAO);
                        }
                        if (landscapeTuple.Item1 != -1) // there exists a landscape orientation for this overlay
                        {
                            landscapeAO.Id = id + "_L";
                            landscapeAO.Orientation = "landscape";
                            landscapeAO.X = landscapeX;
                            landscapeAO.Y = landscapeTuple.Item2;
                            landscapeAO.Width = landscapeWidth;
                            landscapeAO.Height = landscapeHeight;
                            landscapeAO.AudioUrl = ao.AudioUrl;
                            landscapeAO.AutoStart = ao.AutoStart;
                            landscapeAO.AutoStartDelay = ao.AutoStartDelay;
                            article.Pages[landscapeTuple.Item1].AudioOverlays.Add(landscapeAO);
                        }
                        break;
                    case "hyperlink":
                        HyperlinkOverlay ho = parseHyperlinkOverlay(reader);
                        HyperlinkOverlay portraitHO = new HyperlinkOverlay(ho);
                        HyperlinkOverlay landscapeHO = new HyperlinkOverlay(ho);

                        if (portraitTuple.Item1 != -1) // there exists a portrait orientation for this overlay
                        {
                            portraitHO.Id = id + "_P";
                            portraitHO.Orientation = "portrait";
                            portraitHO.X = portraitX;
                            portraitHO.Y = portraitTuple.Item2;
                            portraitHO.Width = portraitWidth;
                            portraitHO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].HyperlinkOverlays.Add(portraitHO);
                        }
                        if (landscapeTuple.Item1 != -1) // there exists a landscape orientation for this overlay
                        {
                            landscapeHO.Id = id + "_L";
                            landscapeHO.Orientation = "landscape";
                            landscapeHO.X = landscapeX;
                            landscapeHO.Y = landscapeTuple.Item2;
                            landscapeHO.Width = landscapeWidth;
                            landscapeHO.Height = landscapeHeight;
                            article.Pages[landscapeTuple.Item1].HyperlinkOverlays.Add(landscapeHO);
                        }
                        break;
                    case "imagepan":
                        ImagepanOverlay portraitIO = new ImagepanOverlay();
                        ImagepanOverlay landscapeIO = new ImagepanOverlay();
                        
                        // get overlay assets before we get the overlays attributes
                        while (reader.Read())
                        {
                            // indicates the end of the asstes
                            if (reader.Name == "anchorPoint")
                            {
                                break;
                            }
                            else if (reader.Name == "overlayAsset")
                            {
                                if (reader.GetAttribute("landscape") == "false")
                                {
                                    portraitIO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                                else if (reader.GetAttribute("landscape") == "true")
                                {
                                    landscapeIO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                            }
                        }

                        ImagepanOverlay io = parseImagepanOverlay(reader);

                        if (portraitTuple.Item1 != -1)  // there exists a portrait orientation for this overlay
                        {
                            portraitIO.Id = id + "_P";
                            portraitIO.Orientation = "portrait";
                            portraitIO.X = portraitX;
                            portraitIO.Y = portraitTuple.Item2;
                            portraitIO.Width = portraitWidth;
                            portraitIO.Height = portraitHeight;
                            portraitIO.AnchorX = io.AnchorX;
                            portraitIO.AnchorY = io.AnchorY;
                            // getting the viewport bounds if they exist
                             while (reader.Read())
                            {
                                if ((reader.Name == "portraitBounds" && reader.NodeType == XmlNodeType.EndElement) || reader.Name == "landscapeBounds")
                                {
                                    break;
                                }
                                else if (reader.Name == "portraitBounds")
                                {
                                    portraitIO = parseImagepanOverlayViewportbounds(reader, portraitIO);
                                } 
                            }
                           
                            article.Pages[portraitTuple.Item1].ImagepanOverlays.Add(portraitIO);
                        }
                        if (landscapeTuple.Item1 != -1)  // there exists a landscape orientation for this overlay
                        {
                            landscapeIO.Id = id + "_L";
                            landscapeIO.Orientation = "landscape";
                            landscapeIO.X = landscapeX;
                            landscapeIO.Y = landscapeTuple.Item2;
                            landscapeIO.Width = landscapeWidth;
                            landscapeIO.Height = landscapeHeight;
                            landscapeIO.AnchorX = io.AnchorX;
                            landscapeIO.AnchorY = io.AnchorY;
                            // getting the viewport bounds if they exist
                            do{
                                if(reader.Name == "initialViewport" && reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }
                                else if (reader.Name == "landscapeBounds")
                                {
                                    landscapeIO = parseImagepanOverlayViewportbounds(reader, landscapeIO);
                                }
                            }while (reader.Read());

                            article.Pages[landscapeTuple.Item1].ImagepanOverlays.Add(landscapeIO);
                        }
                        break;
                    case "slideshow":
                        // shared slideshow overlay attributes
                        SlideshowOverlay so = parseSlideShowOverlay(reader);

                        SlideshowOverlay portraitSO = new SlideshowOverlay(so);
                        SlideshowOverlay landscapeSO = new SlideshowOverlay(so);
                        
                        // getting the overlay assets for each oreintation if there are any
                        do{
                            if (reader.Name == "portraitLayout" || reader.Name == "landscapeLayout")
                            {
                                break;
                            }
                            else if (reader.Name == "overlayAsset")
                            {
                                if (reader.GetAttribute("landscape") == "false")
                                {
                                    portraitSO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                                else if (reader.GetAttribute("landscape") == "true")
                                {
                                    landscapeSO.OverlayAssets.Add(parseOverlayAsset(reader));
                                }
                            }
                        }while (reader.Read());

                        if (portraitTuple.Item1 != -1)  // there is a portrait layout for this overlay
                        {
                            portraitSO.Id = id + "_P";
                            portraitSO.X = portraitX;
                            portraitSO.Y = portraitTuple.Item2;
                            portraitSO.Width = portraitWidth;
                            portraitSO.Height = portraitHeight;
                            portraitSO.Orientation = "portrait";

                            if (reader.Name == "portraitLayout")
                            {
                                portraitSO = parseSlideshowOverlayDisplaybounds(reader, portraitSO);
                                while (reader.Read())
                                {
                                    if (reader.Name == "landscapeLayout" || (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                    else if (reader.Name == "button")
                                    {
                                        portraitSO.OverlayButtons.Add(parseOverlayButton(reader));
                                    }
                                }
                            }
                            article.Pages[portraitTuple.Item1].SlideshowOverlays.Add(portraitSO);
                        }
                        if (landscapeTuple.Item1 != -1)  // there is a landscape layout for this overlay
                        {
                            landscapeSO.Id = id + "_L";
                            landscapeSO.X = landscapeX;
                            landscapeSO.Y = landscapeTuple.Item2;
                            landscapeSO.Width = landscapeWidth;
                            landscapeSO.Height = landscapeHeight;
                            landscapeSO.Orientation = "landscape";

                            if (reader.Name == "landscapeLayout")
                            {
                                landscapeSO = parseSlideshowOverlayDisplaybounds(reader, landscapeSO);
                                while (reader.Read())
                                {
                                    if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        break;
                                    }
                                    else if (reader.Name == "button")
                                    {
                                        landscapeSO.OverlayButtons.Add(parseOverlayButton(reader));
                                    }
                                }
                            }
                            
                            article.Pages[landscapeTuple.Item1].SlideshowOverlays.Add(landscapeSO);
                        }
                        break;
                    case "webview":
                        WebviewOverlay wo = parseWebViewOverlay(reader);
                        WebviewOverlay portraitWO = new WebviewOverlay(wo);
                        WebviewOverlay landscapeWO = new WebviewOverlay(wo);

                        if (portraitTuple.Item1 != -1) // there is a portrait layout for this overlay
                        {
                            portraitWO.Id = id + "_P";
                            portraitWO.Orientation = "portrait";
                            portraitWO.X = portraitX;
                            portraitWO.Y = portraitTuple.Item2;
                            portraitWO.Width = portraitWidth;
                            portraitWO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].WebviewOverlay.Add(portraitWO);
                        }
                        if (landscapeTuple.Item1 != -1) // there is a landscape layout for this overlay
                        {
                            landscapeWO.Id = id + "_L";
                            landscapeWO.Orientation = "landscape";
                            landscapeWO.X = landscapeX;
                            landscapeWO.Y = landscapeTuple.Item2;
                            landscapeWO.Width = landscapeWidth;
                            landscapeWO.Height = landscapeHeight;
                            article.Pages[landscapeTuple.Item1].WebviewOverlay.Add(landscapeWO);
                        }
                        break;
                    case "video":
                        VideoOverlay vo = parseVideoOverlay(reader);
                        VideoOverlay portraitVO = new VideoOverlay(vo);
                        VideoOverlay landscapeVO = new VideoOverlay(vo);

                        if (portraitTuple.Item1 != -1)  // there is a portrait layout for this overlay
                        {
                            portraitVO.Id = id + "_P";
                            portraitVO.Orientation = "portrait";
                            portraitVO.X = portraitX;
                            portraitVO.Y = portraitTuple.Item2;
                            portraitVO.Width = portraitWidth;
                            portraitVO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].VideoOverlays.Add(portraitVO);
                        }
                        if (landscapeTuple.Item1 != -1)  // there is a landscape layout for this overlay
                        {
                            landscapeVO.Id = id + "_L";
                            landscapeVO.Orientation = "landscape";
                            landscapeVO.X = landscapeX;
                            landscapeVO.Y = landscapeTuple.Item2;
                            landscapeVO.Width = landscapeWidth;
                            landscapeVO.Height = landscapeHeight;
                            article.Pages[landscapeTuple.Item1].VideoOverlays.Add(landscapeVO);
                        }

                        break;
                    default:
                        break;
                }
            }
            return article;
        }

        /// <summary>
        /// Calculates which portrait page the overlay is maped to
        /// </summary>
        /// <param name="y">The y cordinate based on the entire article</param>
        /// <param name="pages">The list of pages that this article has</param>
        /// <returns> a Tuple<index of page, page dependant y cordinate</returns>
        public Tuple<int, double> calculatePortraitPage(double y, IList<Page> pages)
        {
            double remainingHeight = y;
            int index = -1;
            foreach (var p in pages)
            {
                if (p.PortraitContentHeight < remainingHeight)
                {
                    remainingHeight -= p.PortraitContentHeight;
                }
                else
                {
                     index = pages.IndexOf(p);
                     break;
                }
            }
            return Tuple.Create(index, remainingHeight);
        }

        /// <summary>
        /// Calculates which landscape page the overlay is maped to
        /// </summary>
        /// <param name="y">The y cordinate based on the entire article</param>
        /// <param name="pages">The list of pages that this article has</param>
        /// <returns> a Tuple<index of page, page dependant y cordinate</returns>
        public Tuple<int, double> calculateLandscapePage(double y, IList<Page> pages)
        {
            double remainingHeight = y;
            int index = -1;
            foreach (var p in pages)
            {
                if (p.LandscapeContentHeight < remainingHeight)
                {
                    remainingHeight -= p.LandscapeContentHeight;
                }
                else
                {
                    index = pages.IndexOf(p);
                    break;
                }
            }
            return Tuple.Create(index, remainingHeight);
        }
        
        /// <returns>The parsed overlay asset</returns>
        public OverlayAsset parseOverlayAsset(XmlReader reader)
        {
            OverlayAsset oa = new OverlayAsset();
            oa.Id = reader.GetAttribute("id");
            oa.Width = Convert.ToDouble(reader.GetAttribute("width"));
            oa.Height = Convert.ToDouble(reader.GetAttribute("height"));
            oa.Tag = reader.GetAttribute("tag") == null ? "" : reader.GetAttribute("tag");

            reader.Read();
            oa.Url = reader.Value;
            return oa;
        }

        /// <returns> The parsed button to be attached to an overlay</returns>
        public OverlayButton parseOverlayButton(XmlReader reader)
        {
            OverlayButton ob = new OverlayButton();
            ob.Id = reader.GetAttribute("defaultID");
            ob.TargetId = reader.GetAttribute("targetID");
            ob.SelectedId = reader.GetAttribute("selectedID");
            ob.StopEnabled = Convert.ToBoolean(reader.GetAttribute("stopEnabled"));
            while (reader.Read())
            {
                if (reader.Name == "button" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.Name == "rectangle" && reader.NodeType != XmlNodeType.EndElement)
                {
                    ob.X = Convert.ToDouble(reader.GetAttribute("x"));
                    ob.Y = Convert.ToDouble(reader.GetAttribute("y"));
                    ob.Height = Convert.ToDouble(reader.GetAttribute("height"));
                    ob.Width = Convert.ToDouble(reader.GetAttribute("width"));
                }
            }

            return ob;
        }

        public SlideshowOverlay parseSlideshowOverlayDisplaybounds(XmlReader reader, SlideshowOverlay so)
        {
            // this works because we know there is a rectangle element tied to this overlay and won't skip to the next overlay
            // from the way this function is called
            if (reader.ReadToFollowing("rectangle"))
            {
                so.DisplayBoundsX = Convert.ToDouble(reader.GetAttribute("x"));
                so.DisplayBoundsY = Convert.ToDouble(reader.GetAttribute("y"));
                so.DisplayBoundsWidth = Convert.ToDouble(reader.GetAttribute("width"));
                so.DisplayBoundsHeight = Convert.ToDouble(reader.GetAttribute("height"));
            }
            return so;
        }

        public ImagepanOverlay parseImagepanOverlayViewportbounds(XmlReader reader, ImagepanOverlay io)
        {
            // this works because we know there is a rectangle element tied to this overlay and won't skip to the next overlay
            // from the way this function is called
            if (reader.ReadToFollowing("rectangle"))
            {
                io.ViewPortBoundsX = Convert.ToDouble(reader.GetAttribute("x"));
                io.ViewPortBoundsY = Convert.ToDouble(reader.GetAttribute("y"));
                io.ViewPortBoundsWidth = Convert.ToDouble(reader.GetAttribute("width"));
                io.ViewPortBoundsHeight = Convert.ToDouble(reader.GetAttribute("height"));
            }
            return io;
        }

        public AudioOverlay parseAudioOverlay(XmlReader reader)
        {
            AudioOverlay ao = new AudioOverlay();

            do{
                if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if(reader.NodeType != XmlNodeType.EndElement)
                {
                    switch(reader.Name)
                    {
                        case "audioUrl":
                            reader.Read();
                            ao.AudioUrl = reader.Value;
                            break;
                        case "autoStart":
                            reader.Read();
                            ao.AutoStart = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStartDelay":
                            reader.Read();
                            ao.AutoStartDelay = Convert.ToDouble(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
            } while (reader.Read());
            return ao;
        }

        public HyperlinkOverlay parseHyperlinkOverlay(XmlReader reader)
        {
            HyperlinkOverlay ho = new HyperlinkOverlay();
            
            while (reader.Read())
            {
                if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if(reader.NodeType != XmlNodeType.EndElement)
                {
                    switch(reader.Name)
                    {
                        case "url":
                            reader.Read();
                            ho.Url = reader.Value;
                            break;
                        case "reqNavConfirm":
                            reader.Read();
                            ho.ReqNavConfirm = Convert.ToBoolean(reader.Value);
                            break;
                        case "openInApp":
                            reader.Read();
                            ho.OpenInApp = Convert.ToBoolean(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return ho;
        }

        public ImagepanOverlay parseImagepanOverlay(XmlReader reader)
        {
            ImagepanOverlay io = new ImagepanOverlay();
            while (reader.Read())
            {
                if (reader.Name == "landscapeBounds" || reader.Name == "portraitLayout")
                {
                    break;
                }
                else if (reader.Name == "point" && reader.NodeType != XmlNodeType.EndElement)
                {
                    io.AnchorX = Convert.ToDouble(reader.GetAttribute("x"));
                    io.AnchorY = Convert.ToDouble(reader.GetAttribute("y"));
                }
            }
            return io;
        }

        public SlideshowOverlay parseSlideShowOverlay(XmlReader reader)
        {
            SlideshowOverlay so = new SlideshowOverlay();
            while (reader.Read())
            {
                if (reader.Name == "overlayAsset" || reader.Name == "landscapeLayout" || reader.Name == "portraitLayout")
                {
                    break;
                }
                else if (reader.NodeType != XmlNodeType.EndElement)
                {
                    switch(reader.Name)
                    {
                        case "drawHighlight":
                            reader.Read();
                            so.DrawHighlight = Convert.ToBoolean(reader.Value);
                            break;
                        case "selectedHighlight":
                            reader.Read();
                            so.SelectedHighlight = Convert.ToDouble(reader.Value);
                            break;
                        case "crossFadeImages":
                            reader.Read();
                            so.CrossFadeImages = Convert.ToBoolean(reader.Value);
                            break;
                        case "crossFadeImagesDelay":
                            reader.Read();
                            so.CrossFadeImagesDelay = Convert.ToDouble(reader.Value);
                            break;
                        case "defaultSelected":
                            reader.Read();
                            so.DefaultSelected = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStart":
                            reader.Read();
                            so.AutoStart = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStartDelay":
                            reader.Read();
                            so.AutoStartDelay = Convert.ToDouble(reader.Value);
                            break;
                        case "autoTransitionDuration":
                            reader.Read();
                            so.AutoTransitionDuration = Convert.ToDouble(reader.Value);
                            break;
                        case "loopCount":
                            reader.Read();
                            so.LoopCount = Convert.ToInt32(reader.Value);
                            break;
                        case "reverseImageOrder":
                            reader.Read();
                            so.ReverseImageOrder = Convert.ToBoolean(reader.Value);
                            break;
                        case "tapEnabled":
                            reader.Read();
                            so.TapEnabled = Convert.ToBoolean(reader.Value);
                            break;
                        case "swipeEnabled":
                            reader.Read();
                            so.SwipeEnabled = Convert.ToBoolean(reader.Value);
                            break;
                        case "swipeStop":
                            reader.Read();
                            so.SwipeStop = Convert.ToBoolean(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return so;
        }

        public WebviewOverlay parseWebViewOverlay(XmlReader reader)
        {
            WebviewOverlay wo = new WebviewOverlay();

            while (reader.Read())
            {
                if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if(reader.NodeType != XmlNodeType.EndElement)
                {
                    switch(reader.Name)
                    {
                        case "webViewUrl":
                            reader.Read();
                            wo.WebViewUrl = reader.Value;
                            break;
                        case "useTransparentBackground":
                            reader.Read();
                            wo.UseTransparentBackground = Convert.ToBoolean(reader.Value);
                            break;
                        case "userInteractionEnabled":
                            reader.Read();
                            wo.UserInteractionEnabled = Convert.ToBoolean(reader.Value);
                            break;
                        case "scaleContentToFit":
                            reader.Read();
                            wo.ScaleContentToFit = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStart":
                            reader.Read();
                            wo.AutoStart = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStartDelay":
                            reader.Read();
                            wo.AutoStartDelay = Convert.ToDouble(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return wo;
        }

        public VideoOverlay parseVideoOverlay(XmlReader reader)
        {
            VideoOverlay vo = new VideoOverlay();

            while (reader.Read())
            {
                if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if(reader.NodeType != XmlNodeType.EndElement)
                {
                    switch(reader.Name)
                    {
                        case "videoUrl":
                            reader.Read();
                            vo.VideoUrl = reader.Value;
                            break;
                        case "playInContext":
                            reader.Read();
                            vo.PlayInContext = Convert.ToBoolean(reader.Value);
                            break;
                        case "showControlsByDefault":
                            reader.Read();
                            vo.ShowControlsByDefault = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStart":
                            reader.Read();
                            vo.AutoStart = Convert.ToBoolean(reader.Value);
                            break;
                        case "autoStartDelay":
                            reader.Read();
                            vo.AutoStartDelay = Convert.ToDouble(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return vo;
        }
    }
}
