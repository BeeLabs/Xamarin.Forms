﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4597, "[Android] ImageCell not loading images and setting ImageSource to null has no effect",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Image)]
	[NUnit.Framework.Category(UITestCategories.ListView)]
	[NUnit.Framework.Category(UITestCategories.UwpIgnore)]
#endif
	public class Issue4597 : TestContentPage
	{
		ImageButton _imageButton;
		Button _button;
		Image _image;
		ListView _listView;

		string _disappearText = "You should see an Image. Clicking this should cause the image to disappear";
		string _appearText = "Clicking this should cause the images to all appear";
		string _theListView = "theListViewAutomationId";
		string _fileName = "coffee.png";
		string _fileNameAutomationId = "CoffeeAutomationId";
		string _uriImage = "https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/Xamarin.Forms.Controls/coffee.png";
		bool _isUri = false;
		string _nextTestId = "NextTest";
		string _activeTestId = "activeTestId";
		string _switchUriId = "SwitchUri";
		string _imageFromUri = "Image From Uri";
		string _imageFromFile = "Image From File";

		protected override void Init()
		{
			Label labelActiveTest = new Label()
			{
				AutomationId = _activeTestId
			};

			_image = new Image() { Source = _fileName, AutomationId = _fileNameAutomationId };
			_button = new Button() { ImageSource = _fileName, AutomationId = _fileNameAutomationId };
			_imageButton = new ImageButton() { Source = _fileName, AutomationId = _fileNameAutomationId };
			_listView = new ListView()
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var cell = new ImageCell();
					cell.SetBinding(ImageCell.ImageSourceProperty, ".");
					cell.AutomationId = _fileNameAutomationId;
					return cell;
				}),
				AutomationId = _theListView,
				ItemsSource = new[] { _fileName },
				HasUnevenRows = true,
				BackgroundColor = Color.Purple
			};

			View[] imageControls = new View[] { _image, _button, _imageButton, _listView };

			Button button = null;
			button = new Button()
			{
				AutomationId = "ClickMe",
				Text = _disappearText,
				Command = new Command(() =>
				{
					if (button.Text == _disappearText)
					{
						_image.Source = null;
						_button.ImageSource = null;
						_imageButton.Source = null;
						_listView.ItemsSource = new string[] { null };
						Device.BeginInvokeOnMainThread(() => button.Text = _appearText);
					}
					else
					{
						_image.Source = _isUri ? _uriImage : _fileName;
						_button.ImageSource = _isUri ? _uriImage : _fileName;
						_imageButton.Source = _isUri ? _uriImage : _fileName;
						_listView.ItemsSource = new string[] { _isUri ? _uriImage : _fileName };
						Device.BeginInvokeOnMainThread(() => button.Text = _disappearText);
					}
				})
			};

			var switchToUri = new Switch
			{
				AutomationId = _switchUriId,
				IsToggled = false
			};
			var sourceLabel = new Label { Text = _imageFromFile };

			switchToUri.Toggled += (_, e) =>
			{
				_isUri = e.Value;

				// reset the images to visible
				button.Text = _appearText;
				button.SendClicked();

				if (_isUri)
					sourceLabel.Text = _imageFromUri;
				else
					sourceLabel.Text = _imageFromFile;
			};

			var switchWithCaption = new Grid() { HeightRequest = 60 };
			switchWithCaption.AddChild(sourceLabel, 0, 0);
			switchWithCaption.AddChild(switchToUri, 1, 0);

			StackLayout layout = null;
			layout = new StackLayout()
			{
				Children =
				{
					labelActiveTest,
					button,
					switchWithCaption,
					new Button()
					{
						Text = "Load Next Image Control to Test",
						Command = new Command(() =>
						{
							var activeImage = layout.Children.Last();
							int nextIndex = imageControls.IndexOf(activeImage) + 1;

							if(nextIndex >= imageControls.Length)
								nextIndex = 0;

							layout.Children.Remove(activeImage);
							layout.Children.Add(imageControls[nextIndex]);
							labelActiveTest.Text = imageControls[nextIndex].GetType().Name;

							// reset the images to visible
							button.Text = _appearText;
							button.SendClicked();
						}),
						AutomationId = _nextTestId
					},
					imageControls[0]
				}
			};

			Content = layout;
			labelActiveTest.Text = imageControls[0].GetType().Name;
		}
#if UITEST

		[Test]
		public void ImageFromFileSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(Image), false);
		}

		[Test]
		public void ImageFromUriSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(Image), true);
		}


		[Test]
		public void ButtonFromFileSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(Button), false);
		}

		[Test]
		public void ButtonFromUriSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(Button), true);
		}


#if !__WINDOWS__
		[Test]
		public void ImageButtonFromFileSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(ImageButton), false);
		}

		[Test]
		public void ImageButtonFromUriSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(ImageButton), true);
		}

		[Test]
		public void ImageCellFromFileSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(ListView), false);
		}

		[Test]
		public void ImageCellFromUriSourceAppearsAndDisappearsCorrectly()
		{
			RunTest(nameof(ListView), true);
		}
#endif

		void RunTest(string testName, bool fileSource)
		{
			SetupTest(testName, fileSource);
			var foundImage = TestForImageVisible();
			SetImageSourceToNull();
			TestForImageNotVisible(foundImage);
		}


		void SetImageSourceToNull()
		{
			RunningApp.Tap("ClickMe");
			RunningApp.WaitForElement(_appearText);
		}

		UITest.Queries.AppResult TestForImageVisible()
		{
			var imageVisible = RunningApp.WaitForElement(_fileNameAutomationId);

			Assert.Greater(imageVisible[0].Rect.Height, 0);
			Assert.Greater(imageVisible[0].Rect.Width, 0);
			return imageVisible[0];
		}

		void TestForImageNotVisible(UITest.Queries.AppResult previousFinding)
		{
			var imageVisible = RunningApp.Query(_fileNameAutomationId);

			if (imageVisible.Length > 0)
			{
				Assert.Less(imageVisible[0].Rect.Height, previousFinding.Rect.Height);
			}
		}

		void SetupTest(string controlType, bool fileSource)
		{
			RunningApp.WaitForElement(_nextTestId);
			string activeTest = null;
			while (RunningApp.Query(controlType).Length == 0)
			{
				activeTest = RunningApp.WaitForElement(_activeTestId)[0].ReadText();
				RunningApp.Tap(_nextTestId);
				RunningApp.WaitForNoElement(activeTest);
			}

			var currentSetting = RunningApp.WaitForElement(_switchUriId)[0].ReadText();

			if (fileSource && RunningApp.Query(_imageFromUri).Length == 0)
				RunningApp.Tap(_switchUriId);
			else if (!fileSource && RunningApp.Query(_imageFromFile).Length == 0)
				RunningApp.Tap(_switchUriId);
		}
#endif
	}
}

//		[Test]
//		public void TestUriImagesDisappearCorrectly()
//		{
//			RunningApp.Tap("SwitchUri");
//			RunningApp.Tap("ClickMe");
//			RunningApp.WaitForElement(_disappearText);
//#if !__WINDOWS__
//			imageCell = RunningApp.Query(app => app.Marked(_theListView).Descendant()).Where(x => x.Class.Contains("Image")).FirstOrDefault();
//#endif

//#if __IOS__
//			Assert.AreEqual(4, elementsBefore.Where(x => x.Class.Contains("Image")).Count());
//#elif __ANDROID__
//			Assert.AreEqual(3, elementsBefore.Length);
//#else
//			Assert.AreEqual(4, elementsBefore.Count());
//#endif


//#if !__WINDOWS__
//			Assert.IsNotNull(imageCell);
//#endif
//			RunningApp.Tap("ClickMe");
//			RunningApp.WaitForElement(_appearText);
//			elementsAfter = RunningApp.WaitForElement(_fileName);
//#if !__WINDOWS__
//			imageCellAfter = RunningApp.Query(app => app.Marked(_theListView).Descendant()).Where(x => x.Class.Contains("Image")).FirstOrDefault();
//			Assert.IsNull(imageCellAfter);
//#endif

//#if __IOS__
//			Assert.AreEqual(0, elementsAfter.Where(x => x.Class.Contains("Image")).Count());
//#elif __ANDROID__
//			foreach (var newElement in elementsAfter)
//			{
//				foreach (var oldElement in elementsBefore)
//				{
//					if (newElement.Class == oldElement.Class)
//					{
//						Assert.IsTrue(newElement.Rect.Height < oldElement.Rect.Height);
//						continue;
//					}
//				}
//			}
//#else
//			//can't validate if images have vanished until this is resolved
//			Assert.Inconclusive(@"https://github.com/xamarin/Xamarin.Forms/issues/4731");
//#endif
//		}

//		[Test]
//		public void TestImagesDisappearCorrectly()
//		{
//			RunningApp.WaitForElement(_fileName);
//			var elementsBefore = RunningApp.WaitForElement(_fileName);
//#if !__WINDOWS__
//			var imageCell = RunningApp.Query(app => app.Marked(_theListView).Descendant()).Where(x => x.Class.Contains("Image")).FirstOrDefault();
//#endif

//#if __IOS__
//			Assert.AreEqual(4, elementsBefore.Where(x => x.Class.Contains("Image")).Count());
//#elif __ANDROID__
//			Assert.AreEqual(3, elementsBefore.Length);
//#else
//			Assert.AreEqual(4, elementsBefore.Count());
//#endif


//#if !__WINDOWS__
//			Assert.IsNotNull(imageCell);
//#endif

//			RunningApp.Tap("ClickMe");
//			RunningApp.WaitForElement(_appearText);
//			var elementsAfter = RunningApp.WaitForElement(_fileName);

//#if !__WINDOWS__
//			var imageCellAfter = RunningApp.Query(app => app.Marked(_theListView).Descendant()).Where(x => x.Class.Contains("Image")).FirstOrDefault();
//			Assert.IsNull(imageCellAfter);
//#endif

//#if __IOS__
//			Assert.AreEqual(0, elementsAfter.Where(x => x.Class.Contains("Image")).Count());
//#elif __ANDROID__
//			foreach (var newElement in elementsAfter)
//			{
//				foreach(var oldElement in elementsBefore)
//				{
//					if(newElement.Class == oldElement.Class)
//					{
//						Assert.IsTrue(newElement.Rect.Height < oldElement.Rect.Height);
//						continue;
//					}
//				}
//			}
//			//can't validate if images have vanished until this is resolved
//			Assert.Inconclusive(@"https://github.com/xamarin/Xamarin.Forms/issues/4731");
//#endif
//		}
//#endif
	//}
