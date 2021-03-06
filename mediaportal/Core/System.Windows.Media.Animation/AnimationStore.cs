#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections;

namespace System.Windows.Media.Animation
{
  internal class AnimationStore
  {
    #region Methods

    public void ApplyAnimationClock(DependencyProperty property, AnimationClock clock, HandoffBehavior handoffBehavior)
    {
      if (_clocks == null)
      {
        _clocks = new Hashtable();
      }

      AnimationClockCollection clocks = _clocks[property.GlobalIndex] as AnimationClockCollection;

      if (clocks == null)
      {
        _clocks[property.GlobalIndex] = clocks = new AnimationClockCollection();
      }

      if (handoffBehavior == HandoffBehavior.SnapshotAndReplace)
      {
        clocks.Clear();
      }

      clocks.Add(clock);
    }

    public void BeginAnimation(DependencyProperty property, AnimationTimeline animation, HandoffBehavior handoffBehavior)
    {
      if (_animations == null)
      {
        _animations = new Hashtable();
      }

      AnimationTimelineCollection timelines = _animations[property.GlobalIndex] as AnimationTimelineCollection;

      if (timelines == null)
      {
        _animations[property.GlobalIndex] = timelines = new AnimationTimelineCollection();
      }

      timelines.Add(animation);
    }

    public object GetValue(DependencyProperty property, object baseValue, PropertyMetadata metadata)
    {
      if (_animations == null)
      {
        return baseValue;
      }

      AnimationTimelineCollection timelines = _animations[property.GlobalIndex] as AnimationTimelineCollection;

      if (timelines == null)
      {
        return baseValue;
      }

      foreach (AnimationClock clock in timelines)
      {
        if (clock.CurrentState == ClockState.Stopped)
        {
          continue;
        }

        baseValue = clock.GetCurrentValue(null, baseValue);
      }

      return baseValue;
    }

    public void RemoveAnimationClock(DependencyProperty property, AnimationClock clock)
    {
      if (_clocks == null)
      {
        throw new InvalidOperationException();
      }

      if (clock == null)
      {
        _clocks.Remove(property);
        return;
      }

      AnimationClockCollection clocks = _clocks[property.GlobalIndex] as AnimationClockCollection;

      clocks.Remove(clock);
    }

    #endregion Methods

    #region Propertiess

    public bool HasAnimatedProperties
    {
      get { return _animations != null && _animations.Count != 0; }
    }

    #endregion Propertiess

    #region Fields

    private Hashtable _clocks;
    private Hashtable _animations;

    #endregion Fields
  }
}