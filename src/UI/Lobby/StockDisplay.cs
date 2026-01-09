using LLGUI;
using UnityEngine;
using UnityEngine.UI;

namespace TourneyMod.UI.Lobby;

internal class StockDisplay
{
    internal RectTransform rectTransform;
    
    private Image imgStocks;
    private Image imgStocksBackdrop;

    private int maxStocks;
    private int stocks;

    internal static void Create(ref StockDisplay stockDisplay, int maxStocks, string name, Transform parent, Vector2 position, Vector2 scale)
    {
        Plugin.LogGlobal.LogInfo(maxStocks);
        stockDisplay = new StockDisplay();
        stockDisplay.rectTransform = LLControl.CreatePanel(parent, name);
        // Assets.GetGameSprite(...)
        stockDisplay.imgStocksBackdrop = LLControl.CreateImage(stockDisplay.rectTransform, JPLELOFJOOH.HPGOLPEOPLN("_spriteStocksBarFill2", maxStocks - 1, false));
        stockDisplay.imgStocks = LLControl.CreateImage(stockDisplay.rectTransform, JPLELOFJOOH.HPGOLPEOPLN("_spriteStocksBarFill", maxStocks - 1, false));

        stockDisplay.rectTransform.anchoredPosition = position;
        stockDisplay.rectTransform.localScale = scale;
    }

    internal void SetMaxStocks(int maxStocks)
    {
        this.maxStocks = Mathf.Clamp(maxStocks, 1, 10);
        imgStocksBackdrop.sprite = JPLELOFJOOH.HPGOLPEOPLN("_spriteStocksBarFill2", this.maxStocks - 1, false);
    }

    internal void SetStocks(int stocks)
    {
        this.stocks = Mathf.Clamp(stocks, 1, maxStocks);
        if (this.stocks < 1) imgStocks.gameObject.SetActive(false);
        else
        {
            imgStocks.gameObject.SetActive(true);
            imgStocks.sprite = JPLELOFJOOH.HPGOLPEOPLN("_spriteStocksBarFill", this.stocks - 1, false);
        }
    }
}