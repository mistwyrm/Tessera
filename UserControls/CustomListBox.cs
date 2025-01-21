using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Tessera.Extensions;
using Tessera.Utilities;
using Tessera.ViewModels;

namespace Tessera.UserControls
{
    public class CustomListBoxItem : ListBoxItem
    {
        private bool _isEditing = false; // Default value set here

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnCustomPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? CustomPropertyChanged;

        protected virtual void OnCustomPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            CustomPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override Type StyleKeyOverride => typeof(ListBoxItem);

        private static readonly Point s_invalidPoint = new Point(double.NaN, double.NaN);
        private Point _pointerDownPoint = s_invalidPoint;


        /// <summary>
        /// Defer selection for PointerPressed to PointerReleased
        /// </summary>
        private bool _deferSelection = false;

        private bool _rightMouseButtonPressed = false;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            _pointerDownPoint = s_invalidPoint;

            if (e.Handled)
                return;

            if (!e.Handled && ItemsControl.ItemsControlFromItemContainer(this) is CustomListBox owner)
            {
                var p = e.GetCurrentPoint(this);

                if (p.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed or
                    PointerUpdateKind.RightButtonPressed)
                {
                    if(p.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonPressed)
                    {
                        _rightMouseButtonPressed = true;
                    }

                    if (p.Pointer.Type == PointerType.Mouse)
                    {
                        // If the pressed point comes from a mouse, perform the selection immediately.
                        if (!this.IsSelected)
                        {
                            // Focus to trigger any LostFocus events
                            this.Focus();
                            e.Handled = owner.UpdateSelectionFromPointerEvent(this, e);
                        }
                        else
                        {
                            _deferSelection = true;
                        }

                    }
                    _pointerDownPoint = p.Position;
                }
            }
        }

        protected async override void OnPointerMoved(PointerEventArgs e)
        {
            var p = e.GetCurrentPoint(this);
            if (ItemsControl.ItemsControlFromItemContainer(this) is CustomListBox owner && owner.DragEnabled && !_rightMouseButtonPressed)
            {
                // Trigger DragDrop if pointer is moved more than DragDistance after a click
                if (!double.IsNaN(_pointerDownPoint.X) && _pointerDownPoint.GetSquaredDistanceTo(e.GetCurrentPoint(this).Position) > Constants.DragDistance)
                {
                    var dataObject = new DataObject();


                    if (owner.SelectedItems is not null)
                    {
                        dataObject.Set(Constants.ListBoxItemFormat, owner.SelectedItems);
                        dataObject.Set(Constants.DragSourceFormat, owner);
                    }

                    var result = await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);

                    if (result == DragDropEffects.Move)
                    {
                        if (owner.DataContext is ViewModelBase ViewModel)
                        {
                            ViewModel.RefreshData();
                        }
                    }

                    // Reset click location and cancel any deferred selection.
                    _deferSelection = false;
                    _pointerDownPoint = s_invalidPoint;
                }
            }
                base.OnPointerMoved(e);
           
            }


        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (!e.Handled &&
                !double.IsNaN(_pointerDownPoint.X) &&
                e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
            {
                var point = e.GetCurrentPoint(this);
                var settings = TopLevel.GetTopLevel(e.Source as Visual)?.PlatformSettings;
                var tapSize = settings?.GetTapSize(point.Pointer.Type) ?? new Size(4, 4);
                var tapRect = new Rect(_pointerDownPoint, new Size())
                    .Inflate(new Thickness(tapSize.Width, tapSize.Height));

                if (new Rect(Bounds.Size).ContainsExclusive(point.Position) &&
                    tapRect.ContainsExclusive(point.Position) &&
                    ItemsControl.ItemsControlFromItemContainer(this) is CustomListBox owner)
                {
                    if (owner.UpdateSelectionFromPointerEvent(this, e))
                    {
                        // As we only update selection from touch/pen on pointer release, we need to raise
                        // the pointer event on the owner to trigger a commit.
                        if (e.Pointer.Type != PointerType.Mouse || _deferSelection == true)
                        {
                            var sourceBackup = e.Source;
                            owner.RaiseEvent(e);
                            e.Source = sourceBackup;
                            _deferSelection = false;
                        }

                        e.Handled = true;
                    }
                }
            }

            _rightMouseButtonPressed = false;
            _pointerDownPoint = s_invalidPoint;
        }
    }

    public class CustomListBox : ListBox
    {
        protected override Type StyleKeyOverride => typeof(ListBox);

        public bool DragEnabled { get; set; } = false;
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new CustomListBoxItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<CustomListBoxItem>(item, out recycleKey);
        }

        internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
        {
            var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
            var toggle = hotkeys is not null && e.KeyModifiers.HasFlag(hotkeys.CommandModifiers);

            return UpdateSelectionFromEventSource(
                source,
                true,
                e.KeyModifiers.HasFlag(KeyModifiers.Shift),
                toggle,
                e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
        }

        public Control? PublicGetContainerFromEventSource(object? eventSource)
        {
            return GetContainerFromEventSource(eventSource);
        }
    }
}
