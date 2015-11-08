﻿/* ------------------------------------------------------------------------- */
///
/// ButtonExtensions.cs
///
/// Copyright (c) 2010 CubeSoft, Inc.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Cube.Pdf.ImageEx.Extensions
{
    /* --------------------------------------------------------------------- */
    ///
    /// Cube.Pdf.ImageEx.Extensions.ButtonExtensions
    ///
    /// <summary>
    /// ボタンに対する拡張メソッド群を定義するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class ButtonExtensions
    {
        /* ----------------------------------------------------------------- */
        ///
        /// UpdateStatus
        /// 
        /// <summary>
        /// ボタンの有効/無効状態を更新し、対応する外観に変更します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void UpdateStatus(this Button button, bool enabled)
        {
            if (enabled) Enable(button);
            else Disable(button);
            button.Enabled = enabled;
        }

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// Enable
        /// 
        /// <summary>
        /// ボタンの外観を有効状態に更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void Enable(Button button)
        {
            if (button.Enabled) return;

            try {
                var colors = (KeyValuePair<Color, Color>)button.Tag;
                button.BackColor = colors.Key;
                button.FlatAppearance.BorderColor = colors.Value;
            }
            catch (Exception) { /* ignore exception */ }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Disable
        /// 
        /// <summary>
        /// ボタンの外観を無効状態に更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void Disable(Button button)
        {
            if (!button.Enabled) return;

            var colors = new KeyValuePair<Color, Color>(button.BackColor, button.FlatAppearance.BorderColor);
            button.Tag = colors;
            button.BackColor = SystemColors.Control;
            button.FlatAppearance.BorderColor = SystemColors.ControlLight;
        }

        #endregion
    }
}