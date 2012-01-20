﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("RootPanel")]
    public class LayoutRoot : LayoutElement, ILayoutContainer, ILayoutRoot
    {
        public LayoutRoot()
        { 
            _floatingWindows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_floatingWindows_CollectionChanged);
            _rightSide = new LayoutAnchorSide() { Parent = this };
            _leftSide = new LayoutAnchorSide() { Parent = this };
            _topSide = new LayoutAnchorSide() { Parent = this };
            _bottomSide = new LayoutAnchorSide() { Parent = this };
        }


        #region RootPanel

        private LayoutPanel _rootPanel = new LayoutPanel();
        public LayoutPanel RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (_rootPanel != value)
                {
                    RaisePropertyChanging("RootPanel");
                    _rootPanel = value;
                    _rootPanel.Parent = this;
                    RaisePropertyChanged("RootPanel");
                }
            }
        }

        #endregion

        #region TopSide

        private LayoutAnchorSide _topSide = null;
        public LayoutAnchorSide TopSide
        {
            get { return _topSide; }
            set
            {
                if (_topSide != value)
                {
                    RaisePropertyChanging("TopSide");
                    _topSide = value;
                    _topSide.Parent = this;
                    RaisePropertyChanged("TopSide");
                }
            }
        }

        #endregion

        #region RightSide

        private LayoutAnchorSide _rightSide;
        public LayoutAnchorSide RightSide
        {
            get { return _rightSide; }
            set
            {
                if (_rightSide != value)
                {
                    RaisePropertyChanging("RightSide");
                    _rightSide = value;
                    _rightSide.Parent = this;
                    RaisePropertyChanged("RightSide");
                }
            }
        }

        #endregion

        #region LeftSide

        private LayoutAnchorSide _leftSide = null;
        public LayoutAnchorSide LeftSide
        {
            get { return _leftSide; }
            set
            {
                if (_leftSide != value)
                {
                    RaisePropertyChanging("LeftSide");
                    _leftSide = value;
                    _leftSide.Parent = this;
                    RaisePropertyChanged("LeftSide");
                }
            }
        }

        #endregion

        #region BottomSide

        private LayoutAnchorSide _bottomSide = null;
        public LayoutAnchorSide BottomSide
        {
            get { return _bottomSide; }
            set
            {
                if (_bottomSide != value)
                {
                    RaisePropertyChanging("BottomSide");
                    _bottomSide = value;
                    _bottomSide.Parent = this;
                    RaisePropertyChanged("BottomSide");
                }
            }
        }

        #endregion

        #region FloatingWindows
        void _floatingWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (LayoutFloatingWindow element in e.NewItems)
                    element.Parent = this;
            }

            if (e.OldItems != null && (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (LayoutFloatingWindow element in e.OldItems)
                    element.Parent = null;
            }
        }

        ObservableCollection<LayoutFloatingWindow> _floatingWindows = new ObservableCollection<LayoutFloatingWindow>();

        public ObservableCollection<LayoutFloatingWindow> FloatingWindows
        {
            get { return _floatingWindows; }
        }
        #endregion

        #region Children
        public IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (RootPanel != null)
                    yield return RootPanel;
                foreach (var floatingWindow in _floatingWindows)
                    yield return floatingWindow;
                if (TopSide != null)
                    yield return TopSide;
                if (RightSide != null)
                    yield return RightSide;
                if (BottomSide != null)
                    yield return RightSide;
                if (LeftSide != null)
                    yield return RightSide;

            }
        }
        #endregion

        #region ActiveContent

        private LayoutContent _activeContent = null;
        public LayoutContent ActiveContent
        {
            get { return _activeContent; }
            set
            {
                if (_activeContent != value)
                {
                    RaisePropertyChanging("ActiveContent");
                    if (_activeContent != null)
                        _activeContent.IsActive = false;
                    _activeContent = value;
                    if (_activeContent != null)
                        _activeContent.IsActive = true;
                    RaisePropertyChanged("ActiveContent");
                }
            }
        }

        #endregion

        #region Manager

        private DockingManager _manager = null;
        public DockingManager Manager
        {
            get { return _manager; }
            internal set
            {
                if (_manager != value)
                {
                    RaisePropertyChanging("Manager");
                    _manager = value;
                    RaisePropertyChanged("Manager");
                }
            }
        }

        #endregion


        #region CollectGarbage

        public void CollectGarbage()
        {
            bool exitFlag = true;

            do
            {
                exitFlag = true;

                foreach (var emptyPane in _rootPanel.Descendents().OfType<LayoutAnchorablePane>().Where(p => p.Children.Count == 0))
                {
                    foreach (var contentReferencingEmptyPane in _rootPanel.Descendents().OfType<LayoutContent>()
                        .Where(c => c.PreviousContainer == emptyPane && c.FindParent<LayoutFloatingWindow>() == null))
                    {
                        contentReferencingEmptyPane.PreviousContainer = null;
                        contentReferencingEmptyPane.PreviousContainerIndex = -1;
                    }

                    if (!_rootPanel.Descendents().OfType<LayoutContent>().Any(c => c.PreviousContainer == emptyPane))
                    {
                        var parentGroup = emptyPane.Parent as ILayoutGroup;
                        parentGroup.RemoveChild(emptyPane);
                        exitFlag = false;
                        break;
                    }
                }

                if (!exitFlag)
                {
                    foreach (var emptyPaneGroup in _rootPanel.Descendents().OfType<LayoutAnchorablePaneGroup>().Where(p => p.Children.Count == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent as ILayoutGroup;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        exitFlag = false;
                        break;
                    }
                
                }


            }
            while (!exitFlag);

        }

        #endregion
    }
}