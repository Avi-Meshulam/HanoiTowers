using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HanoiTowers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string SOURCE_TOWER_NAME = "Source";
        private const string DESTINATION_TOWER_NAME = "Destination";
        private const string TEMP_TOWER_NAME = "Temp";
        private const int DEFAULT_NUMBER_OF_BLOCKS = 5;
        private const int MAX_NUMBER_OF_BLOCKS = 27;
        private const int BLOCK_STEP_SIZE = 20;

        private HanoiTowers _hanoiTowers;
        private IDictionary<string, Tower> _towers;
        private Queue<KeyValuePair<string, string>> _movesQueue;
        private List<string> _towersNamesByPosition = new List<string>
            {
                SOURCE_TOWER_NAME,
                TEMP_TOWER_NAME,
                DESTINATION_TOWER_NAME
            };
        private IList<string> _buildingBlocks = new List<string>();
        private Canvas _cnvMain;
        private TextBlock _tbMovesCounter;
        private int _imageOpenedCounter;
        private int _blocksCount;
        private int _totalMovesCount;
        private bool _isRunning;
        private bool _tbMovesCounterIsLoaded;

        public MainPage()
        {
            this.InitializeComponent();
            InitAppComponents();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void InitAppComponents()
        {
            _isRunning = false;
            _totalMovesCount = 0;
            _hanoiTowers = new HanoiTowers();
            _hanoiTowers.BlockMoved += HanoiTowers_BlockMoved;
            _towers = new Dictionary<string, Tower>();
            _movesQueue = new Queue<KeyValuePair<string, string>>();
            SwitchRootLayoutAvailability(isEnabled: true);
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Escape)
            {
                if (_isRunning)
                {
                    BtnReturn_Click(null, null);
                }
                else
                {
                    Application.Current.Exit();
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_towers.Count > 0)
            {
                return;
            }

            SwitchRootLayoutAvailability(isEnabled: false);

            _isRunning = true;

            _blocksCount = (int)cboBlocksNumber.SelectedItem;

            CreateNewCanvas();
            CreateTowers(_towersNamesByPosition);
            CreateMovesCounter();

            _hanoiTowers.Move(
                SOURCE_TOWER_NAME,
                DESTINATION_TOWER_NAME,
                TEMP_TOWER_NAME,
                _blocksCount);
        }

        private void SwitchRootLayoutAvailability(bool isEnabled)
        {
            cboBlocksNumber.IsEnabled = isEnabled;
            btnStart.IsEnabled = isEnabled;
        }

        private void CreateNewCanvas()
        {
            _cnvMain = new Canvas();
            _cnvMain.Width = rootLayout.ActualWidth;
            _cnvMain.Height = rootLayout.ActualHeight;
            _cnvMain.Background = rootLayout.Background;
            rootLayout.Children.Add(_cnvMain);
        }

        private void CreateTowers(List<string> towersNames)
        {
            int towersCount = towersNames.Count;
            double towersBottom = Math.Floor((_cnvMain.Height / 10) * 9);
            double towersGap = Math.Floor(_cnvMain.Width / (towersCount + 1));
            double towerMaxWidth = Math.Floor(_cnvMain.Width / (towersCount + 2));
            double towerMaxHeight = Math.Floor(_cnvMain.Height - ((_cnvMain.Height - towersBottom) * 2));

            for (int index = 0; index < towersCount; index++)
            {
                Tower tower = new Tower(towersNames[index],
                    new Position(towersBottom, towersGap * (index + 1)));

                _towers.Add(tower.Name, tower);
            }

            CreateTowerBlocks(_towers[SOURCE_TOWER_NAME], towerMaxWidth, towerMaxHeight);
        }

        private void CreateTowerBlocks(Tower tower, double towerWidth, double towerHeight)
        {
            double blockWidth = towerWidth;
            double blockWidthDelta = towerWidth / _blocksCount;
            double blockHeight = towerHeight / _blocksCount;
            int blockImageIndex = Utils.GetRandom(_buildingBlocks.Count);

            _imageOpenedCounter = 0;

            for (int count = 0; count < _blocksCount; count++)
            {
                Position blockPosition =
                    new Position(tower.Position.Top - blockHeight, tower.Position.Center);

                Image blockImage =
                    CreateBlockImage(blockImageIndex, blockWidth, blockHeight, blockPosition);

                var block =
                    new BuildingBlock(blockWidth, blockHeight, blockPosition, blockImage);

                tower.AddBlock(block);

                blockWidth -= blockWidthDelta;
            }
        }

        private Image CreateBlockImage(int index, double width, double height, Position position)
        {
            Image blockImage = new Image();
            blockImage.Source =
                new BitmapImage(new Uri("ms-appx:///Assets/BuildingBlocks/" + _buildingBlocks[index]));
            blockImage.Width = width;
            blockImage.Height = height;
            blockImage.Stretch = Stretch.Fill;
            _cnvMain.Children.Add(blockImage);
            Canvas.SetTop(blockImage, position.Top);
            Canvas.SetLeft(blockImage, position.Center - (width / 2));
            blockImage.ImageOpened += BlockImage_ImageOpened;
            return blockImage;
        }

        private async void BlockImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (++_imageOpenedCounter == _blocksCount)
            {
                while (!_tbMovesCounterIsLoaded)
                {
                    await Task.Delay(100);
                }

                UpdateMovesCounter(counter: 0);

                MoveBlocks();
            }
        }

        private void UpdateMovesCounter(int counter)
        {
            _tbMovesCounter.Text = string.Format("Moves Count: {0}/{1}", counter, _totalMovesCount);
        }

        private void HanoiTowers_BlockMoved(object sender, EventArgs e)
        {
            var args = (BlockMovedEventArgs)e;
            string sourceTower = args.SourceTower;
            string destinationTower = args.DestinationTower;
            _movesQueue.Enqueue(new KeyValuePair<string, string>(sourceTower, destinationTower));
            _totalMovesCount++;
        }

        private async void MoveBlocks()
        {
            string sourceTower;
            string destinationTower;

            int counter = 0;
            while (_movesQueue.Count > 0)
            {
                KeyValuePair<string, string> sourceDestPair = _movesQueue.Dequeue();
                sourceTower = sourceDestPair.Key;
                destinationTower = sourceDestPair.Value;

                double highestPointInRange = GetHighestPointInRange(sourceTower, destinationTower);

                BuildingBlock block = _towers[sourceTower].MoveBlock(_towers[destinationTower]);

                await MoveImage(block.Image, block.Position, highestPointInRange);

                UpdateMovesCounter(++counter);
            }

            ShowReturnButton();
        }

        private double GetHighestPointInRange(string sourceTower, string destTower)
        {
            double highestPosition = double.MaxValue;

            int sourceIndex = _towersNamesByPosition.IndexOf(sourceTower);
            int destIndex = _towersNamesByPosition.IndexOf(destTower);
            int step = (destIndex > sourceIndex) ? 1 : -1;

            int index = sourceIndex;
            do
            {
                index += step;
                Tower tower = _towers[_towersNamesByPosition[index]];
                if (tower.Position.Top < highestPosition)
                {
                    highestPosition = tower.Position.Top;
                }

            } while (Math.Abs(index - destIndex) != 0);

            return highestPosition;
        }

        private async Task MoveImage(Image image, Position newPosition, double highestPointInRange)
        {
            double imgTop = (double)image.GetValue(Canvas.TopProperty);
            if(imgTop >= highestPointInRange)
            {
                await MoveImage(image, highestPointInRange - image.Height, Canvas.TopProperty);
            }
            
            await MoveImage(image, newPosition.Center, Canvas.LeftProperty);
            await MoveImage(image, newPosition.Top, Canvas.TopProperty);
        }

        private async Task MoveImage(Image image, double newPosition, DependencyProperty dp)
        {
            if (dp == Canvas.LeftProperty)
            {
                newPosition -= image.ActualWidth / 2;
            }

            double imgPosition = (double)image.GetValue(dp);

            if (newPosition == imgPosition)
            {
                return;
            }

            double step = newPosition > imgPosition ? BLOCK_STEP_SIZE : -BLOCK_STEP_SIZE;

            do
            {
                if (Math.Abs(newPosition - imgPosition) < BLOCK_STEP_SIZE)
                {
                    step = newPosition - imgPosition;
                }
                imgPosition += step;
                image.SetValue(dp, imgPosition);
                await Task.Delay(1);
            } while (newPosition != imgPosition);
        }

        private void ShowReturnButton()
        {
            Button btnReturn = new Button();
            btnReturn.Name = "btnReturn";
            btnReturn.Content = "Return";
            btnReturn.FontSize = 30;
            btnReturn.HorizontalContentAlignment = HorizontalAlignment.Center;
            btnReturn.VerticalContentAlignment = VerticalAlignment.Center;
            _cnvMain.Children.Add(btnReturn);
            btnReturn.Loaded += BtnReturn_Loaded;
            btnReturn.Click += BtnReturn_Click;
        }

        private void BtnReturn_Loaded(object sender, RoutedEventArgs e)
        {
            Button btnReturn = (Button)sender;
            Canvas.SetTop(btnReturn, (_cnvMain.Height - btnReturn.ActualHeight) / 2);
            Canvas.SetLeft(btnReturn, (_cnvMain.Width - btnReturn.ActualWidth) / 2);
            btnReturn.Focus(FocusState.Keyboard);
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            rootLayout.Children.Remove(_cnvMain);
            InitAppComponents();
        }

        private async void rootLayout_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            InitBlocksCombo();
            await MapAssets();
            btnStart.IsEnabled = true;
        }

        private void InitBlocksCombo()
        {
            for (int counter = 1; counter <= MAX_NUMBER_OF_BLOCKS; counter++)
            {
                cboBlocksNumber.Items.Add(counter);
            }
            cboBlocksNumber.SelectedItem = DEFAULT_NUMBER_OF_BLOCKS;
        }

        private void cboBlocksNumber_Loaded(object sender, RoutedEventArgs e)
        {
            cboBlocksNumber.Focus(FocusState.Keyboard);
        }

        private void CreateMovesCounter()
        {
            _tbMovesCounter = new TextBlock();
            _tbMovesCounter.FontSize = 30;
            _tbMovesCounter.Foreground = new SolidColorBrush(Colors.Red);

            UpdateMovesCounter(counter: 0);

            _tbMovesCounterIsLoaded = false;
            _tbMovesCounter.Loaded += _tbMovesCounter_Loaded;

            _cnvMain.Children.Add(_tbMovesCounter);
        }

        private void _tbMovesCounter_Loaded(object sender, RoutedEventArgs e)
        {
            _tbMovesCounterIsLoaded = true;
            Canvas.SetTop(_tbMovesCounter, _tbMovesCounter.ActualHeight);
            Canvas.SetLeft(_tbMovesCounter, (_cnvMain.Width - _tbMovesCounter.ActualWidth) / 2);
        }

        private async Task MapAssets()
        {
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".png");
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);

            StorageFolder playersFolder = await appInstalledFolder.GetFolderAsync("Assets\\BuildingBlocks");
            var playersFiles = await playersFolder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (StorageFile file in playersFiles)
            {
                _buildingBlocks.Add(file.Name);
            }
        }
    }
}
