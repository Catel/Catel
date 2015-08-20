// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.relations.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels
{
    using Catel.MVVM;
    using NUnit.Framework;

    public partial class ViewModelBaseFacts
    {
        [TestFixture]
        public class DeferValidationUntilFirstSaveCallWithChildViewModels
        {
            public class DeferViewModelBase : ViewModelBase
            {
                public bool DeferValidationUntilFirstSaveValue
                {
                    get { return base.DeferValidationUntilFirstSaveCall; }
                    set { base.DeferValidationUntilFirstSaveCall = value; }
                }
            }

            public class GrantParentViewModel : DeferViewModelBase
            {
                
            }

            public class ParentViewModel : DeferViewModelBase
            {
                
            }

            public class ChildViewModel : DeferViewModelBase
            {
                
            }

            [TestCase]
            public void RetrievesFromParentWhenAttachingViewModelToTree()
            {
                var grantParentVm = new GrantParentViewModel();
                var parentVm = new ParentViewModel();
                var childVm = new ChildViewModel();

                grantParentVm.DeferValidationUntilFirstSaveValue = true;

                ((IRelationalViewModel)grantParentVm).RegisterChildViewModel(parentVm);
                ((IRelationalViewModel)parentVm).SetParentViewModel(grantParentVm);

                ((IRelationalViewModel)parentVm).RegisterChildViewModel(childVm);
                ((IRelationalViewModel)childVm).SetParentViewModel(parentVm);

                Assert.IsTrue(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(childVm.DeferValidationUntilFirstSaveValue);
            }

            [TestCase]
            public void UpdatesChildsWhenUpdatingDeferValidationUntilFirstSave()
            {
                var grantParentVm = new GrantParentViewModel();
                var parentVm = new ParentViewModel();
                var childVm = new ChildViewModel();

                ((IRelationalViewModel)grantParentVm).RegisterChildViewModel(parentVm);
                ((IRelationalViewModel)parentVm).RegisterChildViewModel(childVm);

                Assert.IsFalse(grantParentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsFalse(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsFalse(childVm.DeferValidationUntilFirstSaveValue);

                parentVm.DeferValidationUntilFirstSaveValue = true;

                Assert.IsFalse(grantParentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(parentVm.DeferValidationUntilFirstSaveValue);
                Assert.IsTrue(childVm.DeferValidationUntilFirstSaveValue);
            }
        }
    }
}