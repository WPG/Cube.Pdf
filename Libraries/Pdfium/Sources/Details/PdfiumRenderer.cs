﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/* ------------------------------------------------------------------------- */
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Cube.Pdf.Pdfium
{
    /* --------------------------------------------------------------------- */
    ///
    /// PdfiumRenderer
    ///
    /// <summary>
    /// Provides functionality to render PDF pages via PDFium API.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal static class PdfiumRenderer
    {
        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Render
        ///
        /// <summary>
        /// Executes the rendering with the specified arguments.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static Image Render(IntPtr core, Page page, SizeF size,
            RenderOption options) => Load(core, page.Number, hp =>
        {
            var formCallbacks = new NativeMethods.FPDF_FORMFILLINFO();
            // Depending on whether XFA support is built into the PDFium library, the version
            // needs to be 1 or 2. We don't really care, so we just try one or the other.
            IntPtr form = IntPtr.Zero;
            for (int i = 1; i <= 2; i++)
            {
                formCallbacks.version = i;

                form = NativeMethods.FPDFDOC_InitFormFillEnvironment(core, formCallbacks);
                if (form != IntPtr.Zero)
                    break;
            }

            var width  = (int)size.Width;
            var height = (int)size.Height;
            var degree = GetRotation(page.Delta);
            var flags  = options.GetFlags();

            var bpp  = 4;
            var dest = options.GetBitmap(width, height);
            var data = dest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, dest.PixelFormat);
            var hbm  = NativeMethods.FPDFBitmap_CreateEx(width, height, bpp, data.Scan0, width * bpp);

            NativeMethods.FPDF_RenderPageBitmap(hbm, hp, 0, 0, width, height, degree, flags);
            NativeMethods.FPDF_FFLDraw(form, hbm, hp, 0, 0, width, height, degree, flags);
            NativeMethods.FPDFBitmap_Destroy(hbm);
            dest.UnlockBits(data);

            return dest;
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Render
        ///
        /// <summary>
        /// Executes the rendering with the specified arguments.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Render(IntPtr core, Graphics dest,
            Page page, PointF point, SizeF size,
            RenderOption options) => Load(core, page.Number, hp =>
        {
            options.DrawBackground(e => dest.Clear(e));

            var x      = (int)point.X;
            var y      = (int)point.Y;
            var width  = (int)size.Width;
            var height = (int)size.Height;
            var degree = GetRotation(page.Delta);
            var flags  = options.GetFlags();

            var hdc = dest.GetHdc();
            NativeMethods.FPDF_RenderPage(hdc, hp, x, y, width, height, degree, flags);
            dest.ReleaseHdc(hdc);

            return true;
        });

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// Load
        ///
        /// <summary>
        /// Loads the specified page and invokes the specified function.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static T Load<T>(IntPtr core, int pagenum, Func<IntPtr, T> func)
        {
            if (core == IntPtr.Zero) return default;
            var hp = NativeMethods.FPDF_LoadPage(core, pagenum - 1);
            if (hp == IntPtr.Zero) throw new PdfiumException(PdfiumStatus.PageError);

            try { return func(hp); }
            finally { NativeMethods.FPDF_ClosePage(hp); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetRotation
        ///
        /// <summary>
        /// Gets the rotation angle in degree unit.
        /// </summary>
        ///
        /// <remarks>
        /// Normalizes the degree because the PDFium only supports in
        /// 90-degree units.
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private static int GetRotation(Angle src) => (src + 45).Degree / 90;

        #endregion
    }
}
