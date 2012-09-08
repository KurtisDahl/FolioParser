using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (reader != null)
            {
                if (reader.ReadToFollowing("contentStack"))
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "id")
                        {
                            article.Id = reader.Value;
                        }
                    }
                }

                if(reader.ReadToFollowing("title"))
                {
                    reader.Read();
                    article.Title = reader.Value;
                }
                if (reader.ReadToFollowing("description"))
                {
                    reader.Read();
                    article.Description = reader.Value;
                }
                if (reader.ReadToFollowing("author"))
                {
                    reader.Read();
                    article.Author = reader.Value;
                }
                if (reader.ReadToFollowing("kicker"))
                {
                    reader.Read();
                    article.Kicker = reader.Value;
                }
                if (reader.ReadToFollowing("assets"))
                {
                    article.Pages = parsePages(reader);
                }

                if (reader.ReadToFollowing("overlays"))
                {
                    article = parseOverlays(reader, article);
                }

            //string prevName = "";
            //    while (reader.Read())
            //    {
            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.Element: // The node is an element.
            //                if (reader.Name == "contentStack")
            //                {
            //                    while (reader.MoveToNextAttribute())
            //                    {
            //                        switch (reader.Name)
            //                        {
            //                            case "id":
            //                                article.Id = reader.Value;
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                    }
            //                }
            //                else if (reader.Name == "assets")
            //                {
            //                    article.Pages = parsePages(reader);
            //                }
            //                else if (reader.Name == "overlays")
            //                {
            //                    parseHyperlinkOverlays(reader);
            //                }
            //                else
            //                {
            //                    prevName = reader.Name;
            //                }
            //                break;
            //            case XmlNodeType.Text: //Display the text in each element.
            //                switch (prevName)
            //                {
            //                    case "title":
            //                        article.Title = reader.Value;
            //                        prevName = "";
            //                        break;
            //                    case "description":
            //                        article.Description = reader.Value;
            //                        prevName = "";
            //                        break;
            //                    case "author":
            //                        article.Author = reader.Value;
            //                        break;
            //                    case "kicker":
            //                        article.Kicker = reader.Value;
            //                        break;
            //                    default:
            //                        break;
            //                }

            //                break;
            //            case XmlNodeType.EndElement: //Display the end of the element.
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            }
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
                    else if (reader.GetAttribute("landscape") == "true") //
                    {
                        bool found = false;
                        Page newPage = parseLandscapePage(reader);
                        // find the existing page in the dictionary

                        //Page page = (from p in pages
                        //             where p.Id == newPage.Id
                        //             select p).FirstOrDefault();
                        //if (page != null)
                        //{
                        //    pages[pages.IndexOf(page)] = page + newPage;
                        //}

                        foreach (Page p in pages)
                        {
                            if (p.Id == newPage.Id)
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
                        if (!found)
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
                            page.PortraitContentWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.PortraitScrubberHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        default:
                            break;
                    }
                }
            }
            Regex regex = new Regex("\\d+(_+\\d+)*");
            page.Id = regex.Match(page.PortraitContentUrl).ToString();

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
                            page.LandscapeContentWidth = Convert.ToDouble(reader.GetAttribute("width"));
                            page.LandscapeScrubberHeight = Convert.ToDouble(reader.GetAttribute("height"));
                            break;
                        default:
                            break;
                    }
                }
            }
            Regex regex = new Regex("\\d+(_+\\d+)*");
            page.Id = regex.Match(page.LandscapeContentUrl).ToString();

            return page;
        }

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
                    if (reader.Name == "portraitBounds" && reader.NodeType != XmlNodeType.EndElement)
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
                    else if (reader.Name == "landscapeBounds" && reader.NodeType != XmlNodeType.EndElement)
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
                    else if (reader.Name == "data")
                    {
                        break;
                    }
                }

                switch (type)
                {
                    case "audio":
                        AudioOverlay ao = parseAudioOverlay(reader);
                        AudioOverlay portraitAO = ao;
                        AudioOverlay landscapeAO = ao;

                        if (portraitTuple.Item1 != -1)
                        {
                            portraitAO.Id = id + "_P";
                            portraitAO.Orientation = "portrait";
                            portraitAO.X = portraitX;
                            portraitAO.Y = portraitTuple.Item2;
                            portraitAO.Width = portraitWidth;
                            portraitAO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].AudioOverlays.Add(portraitAO);
                        }
                        if (landscapeTuple.Item1 != -1)
                        {
                            landscapeAO.Id = id + "_L";
                            landscapeAO.Orientation = "landscape";
                            landscapeAO.X = landscapeX;
                            landscapeAO.Y = landscapeTuple.Item2;
                            landscapeAO.Width = landscapeWidth;
                            landscapeAO.Height = landscapeHeight;
                            article.Pages[landscapeTuple.Item1].AudioOverlays.Add(landscapeAO);
                        }
                        break;
                    case "hyperlink":
                        HyperlinkOverlay ho = parseHyperlinkOverlay(reader);
                        HyperlinkOverlay portraitHO = ho;
                        HyperlinkOverlay landscapeHO = ho;
                        if (portraitTuple.Item1 != -1)
                        {
                            portraitHO.Id = id + "_P";
                            portraitHO.Orientation = "portrait";
                            portraitHO.X = portraitX;
                            portraitHO.Y = portraitTuple.Item2;
                            portraitHO.Width = portraitWidth;
                            portraitHO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].HyperlinkOverlays.Add(portraitHO);
                        }
                        if (landscapeTuple.Item1 != -1)
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
                    case "slideshow":
                        // shared slideshow overlay attributes
                        SlideshowOverlay so = parseSlideShowOverlay(reader);
                        SlideshowOverlay portraitSO = so;
                        SlideshowOverlay landscapeSO = so;
                        if (portraitTuple.Item1 != -1)
                        {
                            //create a portrait overlay
                            portraitSO.Id = id + "_P";
                            portraitSO.X = portraitX;
                            portraitSO.Y = portraitTuple.Item2;
                            portraitSO.Width = portraitWidth;
                            portraitSO.Height = portraitHeight;
                            portraitSO.Orientation = "portrait";
                        }
                        if (landscapeTuple.Item1 != -1)
                        {
                            //create a landscape overlay
                            landscapeSO.Id = id + "_L";
                            landscapeSO.X = landscapeX;
                            landscapeSO.Y = landscapeTuple.Item2;
                            landscapeSO.Width = landscapeWidth;
                            landscapeSO.Height = landscapeHeight;
                            landscapeSO.Orientation = "landscape";
                        }

                        while (reader.Read())
                        {
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
                        }

                        if (portraitTuple.Item1 != -1)
                        {
                            if (reader.Name == "portraitLayout")
                            {
                                portraitSO = parseSlideshowOverlayDisplaybounds(reader, portraitSO);
                            }
                            article.Pages[portraitTuple.Item1].SlideshowOverlays.Add(portraitSO);
                        }
                        if (landscapeTuple.Item1 != -1)
                        {
                            if (reader.Name == "landscapeLayout")
                            {
                                landscapeSO = parseSlideshowOverlayDisplaybounds(reader, landscapeSO);
                            }
                            article.Pages[landscapeTuple.Item1].SlideshowOverlays.Add(landscapeSO);
                        }
                        break;
                    case "webview":
                        WebviewOverlay wo = parseWebViewOverlay(reader);
                        WebviewOverlay portraitWO = wo;
                        WebviewOverlay landscapeWO = wo;
                        if (portraitTuple.Item1 != -1)
                        {
                            portraitWO.Id = id + "_P";
                            portraitWO.Orientation = "portrait";
                            portraitWO.X = portraitX;
                            portraitWO.Y = portraitTuple.Item2;
                            portraitWO.Width = portraitWidth;
                            portraitWO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].WebviewOverlay.Add(portraitWO);
                        }
                        if (landscapeTuple.Item1 != -1)
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
                        VideoOverlay portraitVO = vo;
                        VideoOverlay landscapeVO = vo;
                        if (portraitTuple.Item1 != -1)
                        {
                            portraitVO.Id = id + "_P";
                            portraitVO.Orientation = "portrait";
                            portraitVO.X = portraitX;
                            portraitVO.Y = portraitTuple.Item2;
                            portraitVO.Width = portraitWidth;
                            portraitVO.Height = portraitHeight;
                            article.Pages[portraitTuple.Item1].VideoOverlays.Add(portraitVO);
                        }
                        if (landscapeTuple.Item1 != -1)
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

        public OverlayAsset parseOverlayAsset(XmlReader reader)
        {
            OverlayAsset oa = new OverlayAsset();
            oa.Id = reader.GetAttribute("id");
            oa.Width = Convert.ToDouble(reader.GetAttribute("width"));
            oa.Height = Convert.ToDouble(reader.GetAttribute("height"));
            
            reader.Read();
            oa.Url = reader.Value;
            return oa;
        }

        public SlideshowOverlay parseSlideshowOverlayDisplaybounds(XmlReader reader, SlideshowOverlay so)
        {
            if (reader.ReadToFollowing("rectangle"))
            {
                so.DisplayBoundsX = Convert.ToDouble(reader.GetAttribute("x"));
                so.DisplayBoundsY = Convert.ToDouble(reader.GetAttribute("y"));
                so.DisplayBoundsWidth = Convert.ToDouble(reader.GetAttribute("width"));
                so.DisplayBoundsHeight = Convert.ToDouble(reader.GetAttribute("height"));
            }
            return so;
        }

        public AudioOverlay parseAudioOverlay(XmlReader reader)
        {
            AudioOverlay ao = new AudioOverlay();

            while (reader.Read())
            {
                if (reader.Name == "data" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.Name == "audioUrl" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ao.AudioUrl = reader.Value;
                }
                else if (reader.Name == "autoStart" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ao.AutoStart = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "autoStartDelay" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ao.AutoStartDelay = Convert.ToDouble(reader.Value);
                }
            }
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
                else if (reader.Name == "url" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ho.Url = reader.Value;
                }
                else if (reader.Name == "reqNavConfirm" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ho.ReqNavConfirm = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "openInApp" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    ho.OpenInApp = Convert.ToBoolean(reader.Value);
                }
            }
            return ho;
        }

        public SlideshowOverlay parseSlideShowOverlay(XmlReader reader)
        {
            SlideshowOverlay so = new SlideshowOverlay();

            if (reader.ReadToFollowing("drawHighlight"))
            {
                reader.Read();
                so.DrawHighlight = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("selectedHighlight"))
            {
                reader.Read();
                so.SelectedHighlight = Convert.ToDouble(reader.Value);
            }
            if (reader.ReadToFollowing("crossFadeImages"))
            {
                reader.Read();
                so.CrossFadeImages = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("crossFadeImagesDelay"))
            {
                reader.Read();
                so.CrossFadeImagesDelay = Convert.ToDouble(reader.Value);
            }
            if (reader.ReadToFollowing("defaultSelected"))
            {
                reader.Read();
                so.DefaultSelected = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("autoStart"))
            {
                reader.Read();
                so.AutoStart = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("autoStartDelay"))
            {
                reader.Read();
                so.AutoStartDelay = Convert.ToDouble(reader.Value);
            }
            if (reader.ReadToFollowing("autoTransitionDuration"))
            {
                reader.Read();
                so.AutoTransitionDuration = Convert.ToDouble(reader.Value);
            }
            if (reader.ReadToFollowing("loopCount"))
            {
                reader.Read();
                so.LoopCount = Convert.ToInt32(reader.Value);
            }
            if (reader.ReadToFollowing("reverseImageOrder"))
            {
                reader.Read();
                so.ReverseImageOrder = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("tapEnabled"))
            {
                reader.Read();
                so.TapEnabled = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("swipeEnabled"))
            {
                reader.Read();
                so.SwipeEnabled = Convert.ToBoolean(reader.Value);
            }
            if (reader.ReadToFollowing("swipeStop"))
            {
                reader.Read();
                so.SwipeStop = Convert.ToBoolean(reader.Value);
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
                else if (reader.Name == "webViewUrl" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.WebViewUrl = reader.Value;
                }
                else if (reader.Name == "useTransparentBackground" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.UseTransparentBackground = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "userInteractionEnabled" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.UserInteractionEnabled = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "scaleContentToFit" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.ScaleContentToFit = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "autoStart" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.AutoStart = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "autoStartDelay" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    wo.AutoStartDelay = Convert.ToDouble(reader.Value);
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
                else if (reader.Name == "videoUrl" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    vo.VideoUrl = reader.Value;
                }
                else if (reader.Name == "playInContext" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    vo.PlayInContext = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "showControlsByDefault" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    vo.ShowControlsByDefault = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "autoStart" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    vo.AutoStart = Convert.ToBoolean(reader.Value);
                }
                else if (reader.Name == "autoStartDelay" && reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    vo.AutoStartDelay = Convert.ToDouble(reader.Value);
                }
            }
            return vo;
        }
    }
}
