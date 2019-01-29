using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Java.Lang;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views.Animations;

//https://github.com/idunnololz/AnimatedExpandableListView

namespace SgBusApp.Droid {
    public class AnimatedExpandableListView : ExpandableListView {

        /**
         * The duration of the expand/collapse animations
         */
        public static int ANIMATION_DURATION = 150;

        private AnimatedExpandableListAdapter adapter;

        public AnimatedExpandableListView(Context context) : base(context) { }

        public AnimatedExpandableListView(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public AnimatedExpandableListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        /**
         * @see ExpandableListView#setAdapter(ExpandableListAdapter)
         */
        public override void SetAdapter(IExpandableListAdapter adapter) {
            base.SetAdapter(adapter);

            // Make sure that the adapter extends AnimatedExpandableListAdapter
            if (adapter is AnimatedExpandableListAdapter) {
                this.adapter = (AnimatedExpandableListAdapter)adapter;
                this.adapter.SetParent(this);
            } else {
                throw new ClassCastException(adapter.ToString() + " must implement AnimatedExpandableListAdapter");
            }
        }

        /**
         * Expands the given group with an animation.
         * @param groupPos The position of the group to expand
         * @return  Returns true if the group was expanded. False if the group was
         *          already expanded.
         */
        public bool ExpandGroupWithAnimation(int groupPos) {
            bool lastGroup = groupPos == adapter.GroupCount - 1;
            if (lastGroup && Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich) {
                return ExpandGroup(groupPos, true);
            }

            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1) {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex < ChildCount) {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom) {
                        // If the user is not going to be able to see the animation
                        // we just expand the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen

                        // We need to notify the adapter that the group was expanded
                        // without it's knowledge
                        adapter.NotifyGroupExpanded(groupPos);
                        return ExpandGroup(groupPos);
                    }
                }
            }

            // Let the adapter know that we are starting the animation...
            adapter.StartExpandAnimation(groupPos, 0);
            // Finally call expandGroup (note that expandGroup will call
            // notifyDataSetChanged so we don't need to)
            return ExpandGroup(groupPos);
        }

        /**
         * Collapses the given group with an animation.
         * @param groupPos The position of the group to collapse
         * @return  Returns true if the group was collapsed. False if the group was
         *          already collapsed.
         */
        public bool CollapseGroupWithAnimation(int groupPos) {
            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1) {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex >= 0 && childIndex < ChildCount) {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom) {
                        // If the user is not going to be able to see the animation
                        // we just collapse the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen
                        return CollapseGroup(groupPos);
                    }
                } else {
                    // If the group is offscreen, we can just collapse it without an
                    // animation...
                    return CollapseGroup(groupPos);
                }
            }

            // Get the position of the firstChild visible from the top of the screen
            long packedPos = GetExpandableListPosition(FirstVisiblePosition);
            int firstChildPos = GetPackedPositionChild(packedPos);
            int firstGroupPos = GetPackedPositionChild(packedPos);

            // If the first visible view on the screen is a child view AND it's a
            // child of the group we are trying to collapse, then set that
            // as the first child position of the group... see
            // {@link #startCollapseAnimation(int, int)} for why this is necessary
            firstChildPos = firstChildPos == -1 || firstGroupPos != groupPos ? 0 : firstChildPos;

            // Let the adapter know that we are going to start animating the
            // collapse animation.
            adapter.StartCollapseAnimation(groupPos, firstChildPos);

            // Force the listview to refresh it's views
            adapter.NotifyDataSetChanged();
            return IsGroupExpanded(groupPos);
        }

        /**
         * Used for holding information regarding the group.
         */
        public class GroupInfo {
            public bool animating = false;
            public bool expanding = false;
            public int firstChildPosition;

            /**
             * This variable contains the last known height value of the dummy view.
             * We save this information so that if the user collapses a group
             * before it fully expands, the collapse animation will start from the
             * CURRENT height of the dummy view and not from the full expanded
             * height.
             */
            public int dummyHeight = -1;
        }

        /**
         * A specialized adapter for use with the AnimatedExpandableListView. All
         * adapters used with AnimatedExpandableListView MUST extend this class.
         */
        public abstract class AnimatedExpandableListAdapter : BaseExpandableListAdapter {
            private SparseArray<GroupInfo> groupInfo = new SparseArray<GroupInfo>();
            private AnimatedExpandableListView parent;

            public static int STATE_IDLE = 0;
            public static int STATE_EXPANDING = 1;
            public static int STATE_COLLAPSING = 2;

            public void SetParent(AnimatedExpandableListView parent) {
                this.parent = parent;
            }

            public abstract View GetRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent);
            public abstract int GetRealChildrenCount(int groupPosition);

            public GroupInfo GetGroupInfo(int groupPosition) {
                GroupInfo info = groupInfo.Get(groupPosition);
                if (info == null) {
                    info = new GroupInfo();
                    groupInfo.Put(groupPosition, info);
                }
                return info;
            }

            public void NotifyGroupExpanded(int groupPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                info.dummyHeight = -1;
            }

            public void StartExpandAnimation(int groupPosition, int firstChildPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                info.animating = true;
                info.firstChildPosition = firstChildPosition;
                info.expanding = true;
            }

            public void StartCollapseAnimation(int groupPosition, int firstChildPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                info.animating = true;
                info.firstChildPosition = firstChildPosition;
                info.expanding = false;
            }

            public void StopAnimation(int groupPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                info.animating = false;
            }

            public override int GetChildType(int groupPosition, int childPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                if (info.animating) {
                    // If we are animating this group, then all of it's children
                    // are going to be dummy views which we will say is type 0.
                    return 0;
                } else {
                    // If we are not animating this group, then we will add 1 to
                    // the type it has so that no type id conflicts will occur
                    // unless getRealChildType() returns MAX_INT
                    return 1;
                }
            }

            public override int ChildTypeCount {
                get {
                    return 2;
                }
            }

            protected ViewGroup.LayoutParams GenerateDefaultLayoutParams() {
                return new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0);
            }

            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent) {
                GroupInfo info = GetGroupInfo(groupPosition);

                if (info.animating) {
                    // If this group is animating, return the a DummyView...
                    if (convertView is DummyView == false) {
                        convertView = new DummyView(parent.Context);
                        convertView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, 0);
                    }

                    if (childPosition < info.firstChildPosition) {
                        convertView.LayoutParameters.Height = 0;
                        return convertView;
                    }

                    ExpandableListView listView = (ExpandableListView)parent;
                    DummyView dummyView = (DummyView)convertView;

                    // Clear the views that the dummy view draws.
                    dummyView.ClearViews();

                    // Set the style of the divider
                    dummyView.SetDivider(listView.Divider, parent.MeasuredWidth, listView.DividerHeight);

                    // Make measure specs to measure child views
                    int measureSpecW = MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
                    int measureSpecH = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

                    int totalHeight = 0;
                    int clipHeight = parent.Height;

                    int len = GetRealChildrenCount(groupPosition);
                    for (int i = info.firstChildPosition; i < len; i++) {
                        View childView = GetRealChildView(groupPosition, i, (i == len - 1), null, parent);

                        LayoutParams p = (LayoutParams)childView.LayoutParameters;
                        if (p == null) {
                            p = (LayoutParams)GenerateDefaultLayoutParams();
                            childView.LayoutParameters = p;
                        }

                        int lpHeight = p.Height;

                        int childHeightSpec;
                        if (lpHeight > 0) {
                            childHeightSpec = MeasureSpec.MakeMeasureSpec(lpHeight, MeasureSpecMode.Exactly);
                        } else {
                            childHeightSpec = measureSpecH;
                        }

                        childView.Measure(measureSpecW, childHeightSpec);
                        totalHeight += childView.MeasuredHeight;

                        if (totalHeight < clipHeight) {
                            // we only need to draw enough views to fool the user...
                            dummyView.AddFakeView(childView);
                        } else {
                            dummyView.AddFakeView(childView);

                            // if this group has too many views, we don't want to
                            // calculate the height of everything... just do a light
                            // approximation and break
                            int averageHeight = totalHeight / (i + 1);
                            totalHeight += (len - i - 1) * averageHeight;
                            break;
                        }
                    }

                    int state;
                    if (dummyView.Tag == null) {
                        state = STATE_IDLE;
                    } else {
                        state = ((Integer)dummyView.Tag).IntValue();
                    }

                    if (info.expanding && state != STATE_EXPANDING) {
                        ExpandAnimation ani = new ExpandAnimation(dummyView, 0, totalHeight, info);
                        ani.Duration = ANIMATION_DURATION;

                        ExpandableAnimationListener expandAnimationListener = new ExpandableAnimationListener(this, state, groupPosition, dummyView, listView, info);
                        ani.SetAnimationListener(expandAnimationListener);

                        dummyView.StartAnimation(ani);
                        dummyView.Tag = STATE_EXPANDING;


                    } else if (!info.expanding && state != STATE_COLLAPSING) {
                        if (info.dummyHeight == -1) {
                            info.dummyHeight = totalHeight;
                        }

                        ExpandAnimation ani = new ExpandAnimation(dummyView, info.dummyHeight, 0, info);
                        ani.Duration = ANIMATION_DURATION;

                        ExpandableAnimationListener collapseAnimationListener = new ExpandableAnimationListener(this, state, groupPosition, dummyView, listView, info);
                        ani.SetAnimationListener(collapseAnimationListener);

                        dummyView.StartAnimation(ani);
                        dummyView.Tag = STATE_COLLAPSING;
                    }

                    return convertView;
                } else {
                    return GetRealChildView(groupPosition, childPosition, isLastChild, convertView, parent);
                }
            }

            public override int GetChildrenCount(int groupPosition) {
                GroupInfo info = GetGroupInfo(groupPosition);
                if (info.animating) {
                    return info.firstChildPosition + 1;
                } else {
                    return GetRealChildrenCount(groupPosition);
                }
            }
        }

        public class ExpandableAnimationListener : Object, Animation.IAnimationListener {
            private AnimatedExpandableListAdapter adaptor;
            private int state;
            private int groupPosition;
            private DummyView dummyView;
            private ExpandableListView listView;
            private GroupInfo info;


            public ExpandableAnimationListener(AnimatedExpandableListAdapter adaptor, int state, int groupPosition, DummyView dummyView, ExpandableListView listView, GroupInfo info) {
                this.adaptor = adaptor;
                this.state = state;
                this.groupPosition = groupPosition;
                this.dummyView = dummyView;
                this.listView = listView;
                this.info = info;
            }

            public void OnAnimationEnd(Animation animation) {
                if (info.expanding && state != AnimatedExpandableListAdapter.STATE_EXPANDING) {
                    adaptor.StopAnimation(groupPosition);
                    adaptor.NotifyDataSetChanged();
                    dummyView.Tag = AnimatedExpandableListAdapter.STATE_IDLE;
                } else if (!info.expanding && state != AnimatedExpandableListAdapter.STATE_COLLAPSING) {
                    adaptor.StopAnimation(groupPosition);
                    listView.CollapseGroup(groupPosition);
                    adaptor.NotifyDataSetChanged();
                    info.dummyHeight = -1;
                    dummyView.Tag = AnimatedExpandableListAdapter.STATE_IDLE;
                }
            }

            public void OnAnimationRepeat(Animation animation) { }

            public void OnAnimationStart(Animation animation) { }
        }

        public class DummyView : View {
            private List<View> views = new List<View>();
            private Drawable divider;
            private int dividerWidth;
            private int dividerHeight;

            public DummyView(Context context) : base(context) { }

            public void SetDivider(Drawable divider, int dividerWidth, int dividerHeight) {
                if (divider != null) {
                    this.divider = divider;
                    this.dividerWidth = dividerWidth;
                    this.dividerHeight = dividerHeight;

                    divider.SetBounds(0, 0, dividerWidth, dividerHeight);
                }
            }

            /**
             * Add a view for the DummyView to draw.
             * @param childView View to draw
             */
            public void AddFakeView(View childView) {
                childView.Layout(0, 0, Width, childView.MeasuredHeight);
                views.Add(childView);
            }

            protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
                base.OnLayout(changed, left, top, right, bottom);
                int len = views.Count;
                for (int i = 0; i < len; i++) {
                    View v = views[i];
                    v.Layout(left, top, left + v.MeasuredWidth, top + v.MeasuredHeight);
                }
            }

            public void ClearViews() {
                views.Clear();
            }

            protected override void DispatchDraw(Canvas canvas) {
                canvas.Save();
                if (divider != null) {
                    divider.SetBounds(0, 0, dividerWidth, dividerHeight);
                }

                int len = views.Count;
                for (int i = 0; i < len; i++) {
                    View v = views[i];

                    canvas.Save();
                    canvas.ClipRect(0, 0, Width, v.MeasuredHeight);
                    v.Draw(canvas);
                    canvas.Restore();

                    if (divider != null) {
                        divider.Draw(canvas);
                        canvas.Translate(0, dividerHeight);
                    }

                    canvas.Translate(0, v.MeasuredHeight);
                }

                canvas.Restore();
            }
        }

        public class ExpandAnimation : Animation {
            private int baseHeight;
            private int delta;
            private View view;
            private GroupInfo groupInfo;

            public ExpandAnimation(View v, int startHeight, int endHeight, GroupInfo info) {
                baseHeight = startHeight;
                delta = endHeight - startHeight;
                view = v;
                groupInfo = info;

                view.LayoutParameters.Height = startHeight;
                view.RequestLayout();
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t) {
                base.ApplyTransformation(interpolatedTime, t);
                if (interpolatedTime < 1.0f) {
                    int val = baseHeight + (int)(delta * interpolatedTime);
                    view.LayoutParameters.Height = val;
                    groupInfo.dummyHeight = val;
                    view.RequestLayout();
                } else {
                    int val = baseHeight + delta;
                    view.LayoutParameters.Height = val;
                    groupInfo.dummyHeight = val;
                    view.RequestLayout();
                }
            }
        }
    }
}