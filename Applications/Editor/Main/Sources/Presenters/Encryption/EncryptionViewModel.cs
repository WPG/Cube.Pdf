﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
/* ------------------------------------------------------------------------- */
using Cube.Mixin.Observing;
using Cube.Xui;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionViewModel
    ///
    /// <summary>
    /// Represents the ViewModel for a EncryptionWindow instance.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class EncryptionViewModel : DialogViewModel<EncryptionFacade>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the EncryptionViewModel
        /// with the specified arguments.
        /// </summary>
        ///
        /// <param name="callback">Callback when applied.</param>
        /// <param name="src">Encryption object.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        public EncryptionViewModel(Action<Encryption> callback,
            Encryption src,
            SynchronizationContext context
        ) : base(new EncryptionFacade(src), new Aggregator(), context)
        {
            OK.Command = new DelegateCommand(
                () =>
                {
                    Send<CloseMessage>();
                    Facade.Normalize();
                    callback(Facade.Value);
                },
                () => Facade.IsAcceptable()
            )
            .Associate(Enabled)
            .Associate(OwnerPassword)
            .Associate(OwnerConfirm)
            .Associate(OpenPassword)
            .Associate(SharePassword)
            .Associate(UserPassword)
            .Associate(UserConfirm);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Methods
        ///
        /// <summary>
        /// Gets a collection of encryption methods.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<EncryptionMethod> Methods => EncryptionFacade.Methods;

        #region Elements

        /* ----------------------------------------------------------------- */
        ///
        /// Enabled
        ///
        /// <summary>
        /// Gets a menu that encryption is enabled.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Enabled => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuEncryptionEnabled,
            () => Facade.Value.Enabled,
            e  => Facade.Value.Enabled = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Operation
        ///
        /// <summary>
        /// Gets a menu to determine whether that permissions are settable.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Operation => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuOperations,
            () => !OpenPassword.Value || !SharePassword.Value,
            GetInvoker(false)
        ).Associate(OpenPassword).Associate(SharePassword));

        /* ----------------------------------------------------------------- */
        ///
        /// Method
        ///
        /// <summary>
        /// Gets a menu of encryption method.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<EncryptionMethod> Method => Get(() => new BindableElement<EncryptionMethod>(
            () => Properties.Resources.MenuEncryptionMethod,
            () => Facade.Value.Method,
            e  => Facade.Value.Method = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// OwnerPassword
        ///
        /// <summary>
        /// Gets a menu of owner password.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> OwnerPassword => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuOwnerPassword,
            () => Facade.Value.OwnerPassword,
            e  => Facade.Value.OwnerPassword = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// OwnerConfirm
        ///
        /// <summary>
        /// Gets a menu of owner password confirmation.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> OwnerConfirm => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuConfirmPassword,
            () => Facade.OwnerConfirm,
            e  => Facade.OwnerConfirm = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// UserPassword
        ///
        /// <summary>
        /// Gets a menu of user password.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> UserPassword => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuUserPassword,
            () => Facade.Value.UserPassword,
            e  => Facade.Value.UserPassword = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// UserConfirm
        ///
        /// <summary>
        /// Gets a menu of user password confirmation.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> UserConfirm => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuConfirmPassword,
            () => Facade.UserConfirm,
            e  => Facade.UserConfirm = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// OpenPassword
        ///
        /// <summary>
        /// Gets a menu to determine whether to enable user password.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> OpenPassword => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuOpenWithPassword,
            () => Facade.Value.OpenWithPassword,
            e  => Facade.Value.OpenWithPassword = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// SharePassword
        ///
        /// <summary>
        /// Gets a menu to determine whether to share the user password
        /// with the owner password.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> SharePassword => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuSharePassword,
            () => Facade.SharePassword,
            e  => Facade.SharePassword = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowPrint
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow print.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowPrint => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowPrint,
            () => Facade.Value.Permission.Print.IsAllowed(),
            e  => Facade.Value.Permission.Print = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowCopy
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow copying of text and
        /// images.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowCopy => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowCopy,
            () => Facade.Value.Permission.CopyContents.IsAllowed(),
            e  => Facade.Value.Permission.CopyContents = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowModify
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow page insertion,
        /// rotation, or deletion.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowModify => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowAssemble,
            () => Facade.Value.Permission.ModifyContents.IsAllowed(),
            e  => Facade.Value.Permission.ModifyContents = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowAccessibility
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow content extraction
        /// for accessibility.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowAccessibility => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowAccessibility,
            () => Facade.Value.Permission.Accessibility.IsAllowed(),
            e  => Facade.Value.Permission.Accessibility = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowForm
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow input to form fields.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowForm => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowForm,
            () => Facade.Value.Permission.InputForm.IsAllowed(),
            e  => Facade.Value.Permission.InputForm = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// AllowAnnotation
        ///
        /// <summary>
        /// Gets a menu indicating whether to allow creation or editing of
        /// annotations.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> AllowAnnotation => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuAllowAnnotation,
            () => Facade.Value.Permission.ModifyAnnotations.IsAllowed(),
            e  => Facade.Value.Permission.ModifyAnnotations = Facade.GetPermission(e),
            GetInvoker(false)
        ));

        #endregion

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// GetTitle
        ///
        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        ///
        /// <returns>String value.</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected override string GetTitle() => Properties.Resources.TitleEncryption;

        #endregion
    }
}
