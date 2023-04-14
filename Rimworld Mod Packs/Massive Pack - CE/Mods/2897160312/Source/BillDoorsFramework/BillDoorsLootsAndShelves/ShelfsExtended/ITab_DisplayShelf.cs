using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillDoorsLootsAndShelves
{
    public class ITab_DisplayShelf : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(300f, 480f);

        private static Building_Locker locker;

        bool bulk;

        DisplayOffsetInfo infoCache;

        private Building_Locker SelectedLocker
        {
            get
            {
                return Find.Selector.SingleSelectedThing as Building_Locker;
            }
        }

        public override bool IsVisible
        {
            get
            {
                return SelectedLocker != null;// && SelectedLocker.isDisplay && SelectedLocker.tempStorage.Any;
            }
        }

        public ITab_DisplayShelf()
        {
            size = WinSize;
            labelKey = "BDshelves_DisplayTab";
        }

        public override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void FillTab()
        {
            if (locker != SelectedLocker)
            {
                locker = SelectedLocker;
            }
            Rect rect;
            Rect rect2 = (rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f));
            Text.Font = GameFont.Tiny;

            FourCorners(rect2, out Rect TL, out Rect TR, out Rect BL, out Rect BR);

            DrawControlPanel(TL);

            DrawControlPanelII(BL);

            Rect rect3 = TR.BottomPartPixels(50);
            if (Widgets.ButtonText(rect3.LeftHalf(), "BDshelves_Buttom_Copy".Translate(), true, true, true, null))
            {
                infoCache = locker.displayInfo;
            }
            if (Widgets.ButtonText(rect3.RightHalf(), "BDshelves_Buttom_Paste".Translate(), true, true, true, null))
            {
                if (infoCache != null)
                {
                    locker.displayInfo.Assign(infoCache);
                    locker.Notify_ColorChanged();
                }
            }
            Widgets.Label(TR.BottomPartPixels(TR.height - 50), locker.displayInfo.ToString());

            Widgets.Label(BR.TopPartPixels(30), "BDshelves_Buttom_Description".Translate());
            locker.displayInfo.description = Widgets.TextArea(BR.BottomPartPixels(BR.height - 30), locker.displayInfo.description);
        }

        void FourCorners(Rect source, out Rect TL, out Rect TR, out Rect BL, out Rect BR)
        {
            TL = source.TopHalf().LeftHalf();
            TR = source.TopHalf().RightHalf();
            BL = source.BottomHalf().LeftHalf();
            BR = source.BottomHalf().RightHalf();
        }

        void DrawControlPanel(Rect source)
        {
            float width = Math.Min(source.width, source.height) / 3;

            Vector2 size = new Vector2(width, width);

            Vector2 currentPos = new Vector2(source.x, source.y);

            Vector2 right = new Vector2(width, 0);

            Vector2 down = new Vector2(0, width);

            //L A R
            //< B >
            //- V +
            if (Widgets.ButtonText(new Rect(currentPos, size), "L", true, true, true, null))
            {
                locker.displayInfo.drawRot -= bulk ? 12f : 1f; locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(new Rect(currentPos + right, size), "A", true, true, true, null))
            {
                locker.displayInfo.drawOffset.z += bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(new Rect(currentPos + right * 2, size), "R", true, true, true, null))
            {
                locker.displayInfo.drawRot += bulk ? 12f : 1f; locker.Notify_ColorChanged();
            }
            currentPos += down;
            if (Widgets.ButtonText(new Rect(currentPos, size), "<", true, true, true, null))
            {
                locker.displayInfo.drawOffset.x -= bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(new Rect(currentPos + right, size), bulk ? "BDshelves_Buttom_bulk".Translate() : "BDshelves_Buttom_single".Translate(), true, true, true, null))
            {
                bulk = !bulk;
            }
            if (Widgets.ButtonText(new Rect(currentPos + right * 2, size), ">", true, true, true, null))
            {
                locker.displayInfo.drawOffset.x += bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
            currentPos += down;
            if (Widgets.ButtonText(new Rect(currentPos, size), "+", true, true, true, null))
            {
                locker.displayInfo.drawSizeMult += bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(new Rect(currentPos + right, size), "V", true, true, true, null))
            {
                locker.displayInfo.drawOffset.z -= bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(new Rect(currentPos + right * 2, size), "-", true, true, true, null))
            {
                locker.displayInfo.drawSizeMult -= bulk ? 0.1f : 0.01f; locker.Notify_ColorChanged();
            }
        }

        void DrawControlPanelII(Rect source)
        {
            Rect rect = source.BottomHalf();

            if (Widgets.ButtonText(rect.BottomHalf(), "BDshelves_Buttom_Reset".Translate(), true, true, true, null))
            {
                locker.displayInfo.drawOffset = new Vector3();
                locker.displayInfo.drawRot = 0;
                locker.displayInfo.drawSizeMult = 1;
                locker.displayInfo.shouldFlip = false;
                locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(rect.LeftHalf().TopHalf(), "BDshelves_Buttom_flip".Translate(), true, true, true, null))
            {
                locker.displayInfo.shouldFlip = !locker.displayInfo.shouldFlip;
                locker.Notify_ColorChanged();
            }
            if (Widgets.ButtonText(rect.RightHalf().TopHalf(), "BDshelves_Buttom_changestack".Translate(), true, true, true, null))
            {
                locker.displayInfo.useSingleText = !locker.displayInfo.useSingleText;
                locker.Notify_ColorChanged();
            }
        }
    }
}
