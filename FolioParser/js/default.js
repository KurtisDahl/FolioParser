// For an introduction to the Grid template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=232446
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;

    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
                var parser = new FolioParserComponent.Parser();
                parser.parseIssue("IssueFolio.xml");
                parser.parseArticle("ArticleFolio.xml");
                parser.parseArticle("00_0000_NavTo.Folio.xml");
                parser.parseArticle("01_5150_LU0912_sspy04.Folio.xml");
                parser.parseArticle("02_5800_AL0712_Profile_Folder.Folio.xml");
                parser.parseArticle("03_4400_GQ0812_Norway.Folio.xml");
                parser.parseArticle("04_2050_VO0311_Diary.Folio.xml");
                parser.parseArticle("05_1500_SE0712_MOBstyTreySongz.Folio.xml");
                parser.parseArticle("06_3000_TIES_BOWTIE44.Folio.xml");
                parser.parseArticle("09_2100_VO0311_Photo1.Folio.xml");
                parser.parseArticle("10_1300_GQ0612_MRebel.Folio.xml");
                parser.parseArticle("11_2500_BA0812_Foodist.Folio.xml");
                

            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {
                if (nav.location) {
                    nav.history.current.initialPlaceholder = true;
                    return nav.navigate(nav.location, nav.state);
                } else {
                    return nav.navigate(Application.navigator.home);
                }
            }));
        }
    });

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
    };

    app.start();
})();
