using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.UI;
using Stride.UI.Controls;
using System.Linq;

using SR = Stride.Rendering;
using SA = Stride.Animations;

namespace DragAndDrop
{
    public class WindowsManager : StartupScript
    {
        public SpriteFont? Font { get; set; }

        private readonly DragAndDropContainer _mainCanvas = new();
        private CubesGenerator? _cubesGenerator;
        private TextBlock? _textBlock;
        private int _windowId = 1;
        private int _cubesCount = 100;
        private int _totalCubes;
        private TextBlock? _fpsText;

        public override void Start()
        {
            Log.Info("Windows Manager Started");

            Font ??= LoadDefaultFont();

            _cubesGenerator = new CubesGenerator(Services);

            var skyboxEntity = Entity.Scene.Entities.Single(x => x.Name == "Skybox");

            _mainCanvas.Children.Add(CreateMainWindow());
            _mainCanvas.Children.Add(CreateWindow($"Window {_windowId++}", new Vector3(0.02f)));

            Entity.Add(new UIComponent()
            {
                Page = new UIPage() { RootElement = _mainCanvas }
            });
        }

        private SpriteFont? LoadDefaultFont()
            => Game.Content.Load<SpriteFont>("StrideDefaultFont");

        private DragAndDropCanvas CreateMainWindow()
        {
            var canvas = CreateWindow("Main Window");

            _textBlock = GetTextBlock(GetTotal());
            _textBlock.Margin = new Thickness(10, 140, 0, 0);

            canvas.Children.Add(_textBlock);

			_fpsText = GetTextBlock("0 FPS");
			_fpsText.Margin = new Thickness(10, 170, 0, 0);
			BasicCameraController.OnFpsUpdated += (fps) => { _fpsText.Text = fps.ToString("#.0") + " : " + NumChicks; };

			canvas.Children.Add(_fpsText);

            return canvas;
        }

        private string GetTotal() => $"Total Cubes: {_totalCubes}";

        private DragAndDropCanvas CreateWindow(string title, Vector3? position = null)
        {
            var canvas = new DragAndDropCanvas(title, Font!, position);
            canvas.SetPanelZIndex(_mainCanvas.GetNewZIndex());

            var newWindowButton = GetButton("New Window", new Vector2(10, 50));
            newWindowButton.PreviewTouchUp += NewWindowButton_PreviewTouchUp;
            canvas.Children.Add(newWindowButton);

            var generateItemsButton = GetButton("Generate Items", new Vector2(10, 90));
            generateItemsButton.PreviewTouchUp += GenerateItemsButton_PreviewTouchUp;
            canvas.Children.Add(generateItemsButton);

            return canvas;
        }

        private void GenerateItemsButton_PreviewTouchUp(object? sender, TouchEventArgs e)
        {
            GenerateCubes(_cubesCount);

            if (_textBlock is null) return;

            _textBlock.Text = GetTotal();
        }

        private void NewWindowButton_PreviewTouchUp(object? sender, TouchEventArgs e)
            => _mainCanvas.Children.Add(CreateWindow($"Window {_windowId++}"));

        private UIElement GetButton(string title, Vector2 position) => new Button
        {
            Content = GetTextBlock(title),
            BackgroundColor = new Color(100, 100, 100, 200),
            Margin = new Thickness(position.X, position.Y, 0, 0),
        };

        private TextBlock GetTextBlock(string title) => new TextBlock
        {
            Text = title,
            TextColor = Color.White,
            TextSize = 18,
            Font = Font,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        private void GenerateCubes(int count)
        {
			if (loadedMannequinModel == null)
			{
				SR.Model loadedMannequinModel2;
				this.Entity.Add(new GameProfiler());
				loadedMannequinModel = Content.Load<SR.Model>("Models/mannequinModel");
				loadedMannequinModel2 = Content.Load<SR.Model>("Models/mannequinModel");
				_runClip = Content.Load<SA.AnimationClip>("Animations/Run");
			}
			for (int i = 0; i < 20; i++)
				AddChick();

			return;

            if (_cubesGenerator is null) return;

            for (int i = 0; i < count; i++)
            {
                Entity.AddChild(_cubesGenerator.GetCube());
            }

            _totalCubes += count;
        }

		private void AddChick()
		{
			NumChicks++;

			// Create a new model component that references the loaded mannequin model
			var modelComponent = new ModelComponent(loadedMannequinModel);

			// Get a random position near the center of the scene
			var randomPosition = new Vector3(random.Next(-40, 0), 0, random.Next(0, 40));

			// Create a new entity and attach a model component 
			var entity = new Entity(randomPosition, "My new entity with a model component");
			entity.Add(modelComponent);

			var ac = new AnimationComponent();
			ac.Animations.Add("Run", _runClip);
			entity.Add(ac);

			// Add the new entity to the current tutorial scene
			Entity.Scene.Entities.Add(entity);

			_lastAC.Add(ac);

			// We add the spawned entities to a stack to keep track of them
			//spawnedEntities.Push(entity);
		}

		static public int NumChicks;
		private Stride.Animations.AnimationClip _runClip;
		private System.Random random = new System.Random();
		private SR.Model loadedMannequinModel = null;
		static public List<AnimationComponent> _lastAC = new List<AnimationComponent>();

	}
}
