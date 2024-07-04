using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class MemberBioHUD : MonoBehaviour
    {
        public TMP_Text descriptionText;

        [Header("Labels")]
        public TMP_Text fightingArtLabel;
        public TMP_Text pastOccupationLabel;
        public TMP_Text likesLabel;
        public TMP_Text favoriteFoodLabel;
        public TMP_Text mostHatedThingLabel;
        
        [Header("Text")]
        public TMP_Text fightingStyleText;
        public TMP_Text pastOccupationText;
        public TMP_Text likesText;
        public TMP_Text favoriteFoodText;
        public TMP_Text mostHatedThingText;

        public void UpdateLabels()
        {
            if (fightingArtLabel) fightingArtLabel.text = "Fighting Art";
            if (pastOccupationLabel) pastOccupationLabel.text = "Past Occupation";
            if (likesLabel) likesLabel.text = "Likes";
            if (favoriteFoodLabel) favoriteFoodLabel.text = "Favorite Food";
            if (mostHatedThingLabel) mostHatedThingLabel.text = "Most Hated Thing";
        }
        public void UpdateInfo(PartyMember member)
        {
            if (member == null) return;
            if (descriptionText) descriptionText.text = member.GetJob().GetDescription();

            if (fightingStyleText) fightingStyleText.text = "Fighting Art";
            if (pastOccupationText) pastOccupationText.text = "Past Occupation";
            if (likesText) likesText.text = "Likes";
            if (favoriteFoodText) favoriteFoodText.text = "Favorite Food";
            if (mostHatedThingText) mostHatedThingText.text = "Most Hated Thing";
        }
    }
}

