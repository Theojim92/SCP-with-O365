using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3
{
    public class Band
    {
        BandTile myTile;
        Guid tileId = new Guid("D0BAB7A8-FFDC-43C3-B995-87AFB2A43387");
        PageData page;

        public Band()
        {
            CreateTile();
        }
        
        public async void CreateTile()
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    //this.viewModel.StatusMessage = "no paired band";
                    return;
                }
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    //this.viewModel.StatusMessage = "connected";
                    myTile = new BandTile(tileId)
                    {
                        IsBadgingEnabled = true,
                        Name = "Sense",
                        TileIcon = await LoadIcon("ms-appx:///Pictures/TileLogo.png"),
                        SmallIcon = await LoadIcon("ms-appx:///Pictures/SmallLogo.png")
                    };
                    this.CreateBoolPoll();
                    this.CreateRatePoll();
                    this.Message();
                    await bandClient.TileManager.AddTileAsync(myTile);
                    //this.viewModel.StatusMessage = "new tile created";
                }
            }
            catch (BandException ex)
            {
               // this.viewModel.StatusMessage = ex.ToString();
            }
        }
        public async void SendMessage()
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    return;
                }
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    var notifictionManager = bandClient.NotificationManager;
                    // send a notification to the Band with a dialog as well as a page
                    await notifictionManager.SendMessageAsync(
                        tileId,
                        "New Poll Request",
                        "Would you like to answer?",
                        DateTime.Now,
                        MessageFlags.None);
                    await notifictionManager.VibrateAsync(VibrationType.NotificationOneTone);
                }
            }
            catch (BandException ex)
            {
                //this.viewModel.StatusMessage = ex.ToString();
            }
        }

        public async void CreatePoll(string require)
        {
            Guid pageId = new Guid();
            Boolean answered = false;
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                string num = null;
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    //this.viewModel.StatusMessage = "no paired band";
                    return;
                }
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    if (require == "bool")
                    {
                        page = new PageData(
                            pageId,
                            0,
                            new TextBlockData(1, "A bool poll"),
                            new WrappedTextBlockData(2, "Do you like me?"),
                            new TextButtonData(3, "yes"),
                            new TextButtonData(4, "no"));
                        //this.viewModel.StatusMessage = "bool poll created, waiting for response";
                    }
                    if (require == "rate")
                    {
                        page = new PageData(
                            pageId,
                            1,
                            new TextBlockData(1, "A rate poll"),
                            new WrappedTextBlockData(2, "Like me? Give me a score 5!"),
                            new TextButtonData(3, "1"),
                            new TextButtonData(4, "2"),
                            new TextButtonData(5, "3"),
                            new TextButtonData(6, "4"),
                            new TextButtonData(7, "5"));
                       // this.viewModel.StatusMessage = "rate poll created, waiting for response";
                    }
                   
                    await bandClient.TileManager.SetPagesAsync(myTile.TileId, page);

                    await bandClient.NotificationManager.SendMessageAsync(
                       tileId,
                       "New Poll Request",
                       "Would you like to answer?",
                       DateTime.Now,
                       MessageFlags.ShowDialog);
                    await bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationOneTone);

                    TaskCompletionSource<bool> finishEvent = new TaskCompletionSource<bool>();

                    await bandClient.TileManager.StartReadingsAsync();

                    bandClient.TileManager.TileOpened += (s, args) =>
                    {
                    };
                    bandClient.TileManager.TileButtonPressed += (s, args) =>
                    {
                        num = args.TileEvent.ElementId.ToString();
                        answered = true;
                        EventHandler_TileButtonPressed(s, args, bandClient, pageId, num); 
                    };
                    bandClient.TileManager.TileClosed += (s, args) =>
                    {
                        if (answered == true)
                        {
                            EventHandler_TileClosed(s, args, bandClient);
                            finishEvent.TrySetResult(true);
                        }
                    };

                    await finishEvent.Task;

                    // Stop listening for Tile events.
                    await bandClient.TileManager.StopReadingsAsync();

                    //await bandClient.NotificationManager.SendMessageAsync(tileId, "Sense Team", "Thanks for answering!", DateTimeOffset.Now, MessageFlags.ShowDialog);

                    //this.viewModel.StatusMessage = "finished poll";
                }
            }
            catch (BandException ex)
            {
                //this.viewModel.StatusMessage = ex.ToString();
            }
        }
        public async Task<string> Heart_Rate()
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                     return "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    if (!(bandClient.SensorManager.HeartRate.GetCurrentUserConsent() == UserConsent.Granted))
                    {
                        await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                        return "Access to the heart rate sensor is denied.";
                    }
                    else
                    {
                        List<String> data = new List<String>();
                        int samplesReceived = 0; // the number of HeartRate samples received
                        // Subscribe to HeartRate data.
                        bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => 
                        {
                            var q = args.SensorReading.Quality;
                            var hr = args.SensorReading.HeartRate;
                            data.Add("Quanlity:" + q + ", Heart Rate:" + hr); 
                            samplesReceived++;
                        };
                        await bandClient.SensorManager.HeartRate.StartReadingsAsync();

                        // Receive HeartRate data for a while, then stop the subscription.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await bandClient.SensorManager.HeartRate.StopReadingsAsync();

                        string hr_data =string.Format("{0} Heart Rate Data Samples:\n", samplesReceived);

                        foreach (var r in data)
                        {
                            hr_data = hr_data + r + "\n";
                        }

                        return hr_data;
                    }
                }
            }
            catch (Exception ex)
            {
               return ex.ToString();
            }
        }
        public async void Remove_Tile()
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    //this.viewModel.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    IEnumerable<BandTile> tiles = await bandClient.TileManager.GetTilesAsync();

                    foreach (var t in tiles)
                    {
                        await bandClient.TileManager.RemoveTileAsync(t);
                    }

                    //this.viewModel.StatusMessage = "Done. All tiles are deleted";
                }
            }
            catch (Exception ex)
            {
                //this.viewModel.StatusMessage = ex.ToString();
            }
        }
        private async Task<BandIcon> LoadIcon(string uri)
        {
            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }
        private void CreateBoolPoll()
        {
            TextBlock nameTextBlock = new TextBlock() { Color = Colors.Blue.ToBandColor(), ElementId = 1, Rect = new PageRect(0, 0, 200, 30) };
            WrappedTextBlock questionBlock = new WrappedTextBlock() { ElementId = 2, Rect = new PageRect(0, 25, 200, 60) };
            TextButton button1 = new TextButton() { ElementId = 3, Rect = new PageRect(0, 0, 100, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            TextButton button2 = new TextButton() { ElementId = 4, Rect = new PageRect(0, 0, 100, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            FlowPanel panel1 = new FlowPanel(nameTextBlock, questionBlock)
            {
                Orientation = FlowPanelOrientation.Vertical,
                Rect = new PageRect(0, 0, 200, 102)
            };
            FlowPanel panel2 = new FlowPanel(button1, button2)
            {
                Orientation = FlowPanelOrientation.Horizontal,
                Rect = new PageRect(0, 0, 200, 102)
            };
            ScrollFlowPanel panelBool = new ScrollFlowPanel(panel1, panel2)
            {
                Orientation = FlowPanelOrientation.Horizontal,
                Rect = new PageRect(0, 0, 200, 102),
                ScrollBarColorSource = ElementColorSource.BandBase
            };
            PageLayout pageLayout = new PageLayout(panelBool);
            myTile.PageLayouts.Add(pageLayout);
        }
        private void CreateRatePoll()
        {
            TextBlock nameTextBlock = new TextBlock() { Color = Colors.Blue.ToBandColor(), ElementId = 1, Rect = new PageRect(0, 0, 200, 30) };
            WrappedTextBlock questionBlock = new WrappedTextBlock() { ElementId = 2, Rect = new PageRect(0, 0, 200, 70) };
            TextButton button1 = new TextButton() { ElementId = 3, Rect = new PageRect(0, 0, 40, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            TextButton button2 = new TextButton() { ElementId = 4, Rect = new PageRect(0, 0, 40, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            TextButton button3 = new TextButton() { ElementId = 5, Rect = new PageRect(0, 0, 40, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            TextButton button4 = new TextButton() { ElementId = 6, Rect = new PageRect(0, 0, 40, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            TextButton button5 = new TextButton() { ElementId = 7, Rect = new PageRect(0, 0, 40, 100), PressedColor = new BandColor(0xFF, 0x00, 0x00) };
            FlowPanel panel1 = new FlowPanel(nameTextBlock, questionBlock)
            {
                Orientation = FlowPanelOrientation.Vertical,
                Rect = new PageRect(0, 0, 200, 102)
            };
            FlowPanel panel2 = new FlowPanel(button1, button2, button3, button4, button5)
            {
                Orientation = FlowPanelOrientation.Horizontal,
                Rect = new PageRect(0, 0, 200, 102)
            };
            ScrollFlowPanel panelRate = new ScrollFlowPanel(panel1, panel2)
            {
                Orientation = FlowPanelOrientation.Horizontal,
                Rect = new PageRect(0, 0, 200, 102),
                ScrollBarColorSource = ElementColorSource.BandBase
            };
            PageLayout pageLayout = new PageLayout(panelRate);
            myTile.PageLayouts.Add(pageLayout);
        }
        private void Message()
        {
            TextBlock nameTextBlock = new TextBlock() { Color = Colors.Blue.ToBandColor(), ElementId = 1, Rect = new PageRect(0, 0, 200, 30) };
            WrappedTextBlock questionBlock = new WrappedTextBlock() { ElementId = 2, Rect = new PageRect(0, 0, 200, 60) };
            ScrollFlowPanel panelRate = new ScrollFlowPanel(nameTextBlock, questionBlock)
            {
                Orientation = FlowPanelOrientation.Vertical,
                Rect = new PageRect(0, 0, 200, 102),
                ScrollBarColorSource = ElementColorSource.BandBase
            };
            PageLayout pageLayout = new PageLayout(panelRate);
            myTile.PageLayouts.Add(pageLayout);
        }
        void EventHandler_TileOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> e, IBandClient bandClient)
        { // This method is called when the user taps our Band tile. // 
            // e.TileEvent.TileId is the tile’s Guid.
            // e.TileEvent.Timestamp is the DateTimeOffset of the event. // 
            // handle the event 
            //await bandClient.TileManager.StartReadingsAsync();
        }
        async void EventHandler_TileClosed(object sender, BandTileEventArgs<IBandTileClosedEvent> e, IBandClient bandClient)
        { // This method is called when the user exits our Band tile. //
          // e.TileEvent.TileId is the tile’s Guid. 
          // e.TileEvent.Timestamp is the DateTimeOffset of the event. // 
          // handle the event }
            await bandClient.TileManager.RemovePagesAsync(tileId);
        }
        async void EventHandler_TileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e, IBandClient bandClient, Guid pageId, String num)
        { // This method is called when the user presses the
            // button in our tile’s layout. //
            // e.TileEvent.TileId is the tile’s Guid. 
            // e.TileEvent.Timestamp is the DateTimeOffset of the event. 
            // e.TileEvent.PageId is the Guid of our page with the button.
            // e.TileEvent.ElementId is the value assigned to the button 
            page = new PageData(
                            pageId,
                            2,
                            new TextBlockData(1, "sense team"),
                            new WrappedTextBlockData(2, "Thanks for your answer"));
            await bandClient.TileManager.SetPagesAsync(myTile.TileId, page);
        }
    }
}
