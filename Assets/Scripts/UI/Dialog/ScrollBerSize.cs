using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBerSize : ScrollRect
{
    override protected void LateUpdate()
    {
        base.LateUpdate();
        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0.1f;
        }
    }

    override public void Rebuild(CanvasUpdate executing)
    {
        base.Rebuild(executing);
        if (this.verticalScrollbar)
        {
            this.verticalScrollbar.size = 0.1f;
        }
    }
}
