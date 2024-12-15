using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ResultsScreenHUD : MonoBehaviour
    {
        protected UIMenu currentMenu = null;
        public GameObject descriptionBox;
        public QuoteBoxHUD quoteBox;
        public UIMenu levelUpMenu;
        public UIMenu levelUpOverviewMenu;
        public UIMenu rewardsMenu;
        

        public bool isFinished = true;
        public virtual IEnumerator ShowResults()
        {
            isFinished = false;

            gameObject?.SetActive(true);
            descriptionBox?.SetActive(true);
            if (quoteBox) quoteBox.gameObject.SetActive(true);

            levelUpMenu.OpenMenu();

            currentMenu = levelUpMenu;

            while (!isFinished) { yield return null; }
            yield break;
        }
        public virtual void NextMenu()
        {
            if (currentMenu == levelUpMenu)
            {
                levelUpMenu.CloseMenu();
                currentMenu = levelUpOverviewMenu;
                levelUpOverviewMenu.OpenMenu();
                
                return;
            }
            else if (currentMenu == levelUpOverviewMenu)
            {
                levelUpOverviewMenu.CloseMenu();
                currentMenu = rewardsMenu;
                rewardsMenu.OpenMenu();
                
            }
            else
            {
                rewardsMenu.CloseMenu();
                EndResults();
            }
        }
        public void SetWinQuote()
        {
            if (!quoteBox) return;
            var member = PlayerData.instance.GetRandomActivePartyMember();
            quoteBox.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomWinQuote());
        }
        public void SetLevelUpQuote(PartyMember member)
        {
            if (!quoteBox) return;
            quoteBox.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomLevelUpQuote());
        }
        public void SetDropQuote()
        {
            if (!quoteBox) return;
            var member = PlayerData.instance.GetRandomActivePartyMember();
            quoteBox.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomDropsQuote());
        }
        public virtual void EndResults()
        {
            descriptionBox?.SetActive(false);
            if (quoteBox) quoteBox.gameObject.SetActive(false);
            isFinished = true;
        }
    }
}

