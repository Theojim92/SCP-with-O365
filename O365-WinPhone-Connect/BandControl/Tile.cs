using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band.Tiles.Pages;
using Microsoft.Band.Personalization;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class Tile
    {
        public Tile(Guid tileId)
        {
            Id = tileId;
            PageLayouts = new List<PageLayout>();
            this.CreateBoolPoll();
            this.CreateRatePoll();
            this.Message();
            PageImages = new List<BandImage>();
        }
        public Tile(Guid tileId, string name, BandImage icon, BandImage smallIcon) : this(tileId)
        {
            Name = name;
            Icon = icon;
            SmallIcon = smallIcon;
        }
        public Guid Id { get; private set;  }
        public string Name { get; set; }
        public BandImage Icon { get; set; }
        public BandImage SmallIcon { get; set; }
        public List<PageLayout> PageLayouts { get; private set; }
        public List<BandImage> PageImages { get; private set; }
        public bool IsScreenTimeoutDisabled { get; set; }

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
            PageLayouts.Add(pageLayout);
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
            PageLayouts.Add(pageLayout);
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
            PageLayouts.Add(pageLayout);
        }
    }
}
