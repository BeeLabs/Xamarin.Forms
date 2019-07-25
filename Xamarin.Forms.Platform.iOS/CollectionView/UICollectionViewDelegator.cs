﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class UICollectionViewDelegator : UICollectionViewDelegateFlowLayout
	{
		public ItemsViewLayout ItemsViewLayout { get; private set; }
		public ItemsViewController ItemsViewController { get; private set; }
		public SelectableItemsViewController SelectableItemsViewController
		{
			get => ItemsViewController as SelectableItemsViewController;
		}
		public CarouselViewController CarouselViewController { get; set; }

		public GroupableItemsViewController GroupableItemsViewController
		{
			get => ItemsViewController as GroupableItemsViewController;
		}

		public UICollectionViewDelegator(ItemsViewLayout itemsViewLayout, ItemsViewController itemsViewController)
		{
			ItemsViewLayout = itemsViewLayout;
			ItemsViewController = itemsViewController;
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(UIEdgeInsets);
			}

			return ItemsViewLayout.GetInsetForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, layout, section);
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemSelected(collectionView, indexPath);
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemDeselected(collectionView, indexPath);
		}

		public override void CellDisplayingEnded(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			ItemsViewController.PrepareCellForRemoval(cell);
		}

		public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForHeader(collectionView, layout, section);
		}

		public override CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForFooter(collectionView, layout, section);
		}
		
		public override void Scrolled(UIScrollView scrollView)
		{
			CarouselViewController?.Scrolled(scrollView);
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			CarouselViewController?.DecelerationEnded(scrollView);
		}

		public override void DecelerationStarted(UIScrollView scrollView)
		{
			CarouselViewController?.DecelerationStarted(scrollView);
		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			CarouselViewController?.DraggingStarted(scrollView);
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			CarouselViewController?.DraggingEnded(scrollView, willDecelerate);
		}

		public override void ScrollAnimationEnded(UIScrollView scrollView)
		{
			CarouselViewController?.ScrollAnimationEnded(scrollView);
		}
	}
}