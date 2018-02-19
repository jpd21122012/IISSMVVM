using System;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Notifications;

using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace IISSMVVM.Services
{
    internal partial class LiveTileService
    {
        // More about Live Tiles Notifications at https://docs.microsoft.com/windows/uwp/controls-and-patterns/tiles-and-notifications-sending-a-local-tile-notification
        public void SampleUpdate()
        {
            // Construct the tile content
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Intelligent Identificator Security System",
                                    HintStyle = AdaptiveTextStyle.Title,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = "Welcome!!!",
                                    HintStyle = AdaptiveTextStyle.SubtitleSubtle,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Intelligent Identificator Security System",
                                    HintStyle = AdaptiveTextStyle.Title,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = "Welcome!!!",
                                    HintStyle = AdaptiveTextStyle.SubtitleSubtle,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup() { HintWeight = 1 },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 2,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = "Assets/StoreLogo.scale-400.png",
                                                    HintCrop = AdaptiveImageCrop.Circle
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup() { HintWeight = 1 }
                                    }
                                },
                                new AdaptiveText()
                                {
                                    Text = "Intelligent Identificator Security System",
                                    HintStyle = AdaptiveTextStyle.Title,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = "Welcome!!!",
                                    HintStyle = AdaptiveTextStyle.SubtitleSubtle,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    }
                }
            };

            // Then create the tile notification
            var notification = new TileNotification(content.GetXml());
            UpdateTile(notification);
        }

        public async Task SamplePinSecondaryAsync(string pageName)
        {
            // TODO WTS: Call this method to Pin a Secondary Tile from a page.
            // You also must implement the navigation to this specific page in the OnLaunched event handler on App.xaml.cs
            var tile = new SecondaryTile(DateTime.Now.Ticks.ToString());
            tile.Arguments = pageName;
            tile.DisplayName = pageName;
            tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.scale-200.png");
            tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;
            await PinSecondaryTileAsync(tile);
        }
    }
}
