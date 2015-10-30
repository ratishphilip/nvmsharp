#region File Header

// -------------------------------------------------------------------------------
// 
// This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
//
// Author: Ratish Philip
// 
// WPFSpark v1.2
//
// -------------------------------------------------------------------------------

#endregion

namespace WPFSpark
{
    /// <summary>
    /// Defines the various modes a SparkWindow can be in.
    /// </summary>
    public enum WindowMode
    {
        // Fullscreen with Minimize and Close button
        Pane = 0,
        // Fullscreen with close button
        PaneCanClose = 1,
        // Non-Fullscreen, fixed-size with Close button
        CanClose = 2,
        // Non-Fullscreen, fixed-size with Minimize and Close button
        CanMinimize = 3,
        // Non-Fullscreen, fixed-size with Minimize, Maximize and Close button
        CanMaximize = 4,
        // Child Window (Non-Fullscreen, fixed-size) with no system buttons
        ChildWindow = 5
    }
}
