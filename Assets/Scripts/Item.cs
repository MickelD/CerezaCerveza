using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ItemBase
{
    [System.Serializable] private enum ItemEffect
    {
        HealthPotion,
        SteelShield,
        RubberBoots,
        HeavyCrate,
        HeartLocket,
        Crown,
        GoldenAmulet,
        CommonCherry,
        DamageCristal
    }
    [SerializeField] private ItemEffect itemEffect;

    public override void OnEquipThisItem(PlayerManager player)
    {
        switch (itemEffect)
        {
            case ItemEffect.HealthPotion:
                player.SetCurrentHealth(player.currentPlayerHP + 25, player.maxPlayerHP);
                break;
            case ItemEffect.SteelShield:
                player.SetArmor(player.playerArmor + 0.15f);
                break;
            case ItemEffect.RubberBoots:
                player.SetSpeed(player.playerSpeed + 1f);
                break;
            case ItemEffect.HeavyCrate:
                player.SetSpeed(player.playerSpeed - 1f);
                player.SetArmor(player.playerArmor + 0.3f);
                break;
            case ItemEffect.HeartLocket:
                player.SetCurrentHealth(player.currentPlayerHP, player.maxPlayerHP + 20);
                break;
            case ItemEffect.Crown:
                player.SetCurrentHealth(player.currentPlayerHP + 15, player.maxPlayerHP + 10);
                player.SetArmor(player.playerArmor + 0.1f);
                player.SetSpeed(player.playerSpeed + 0.25f);
                player.playerDamage += 5;
                break;
            case ItemEffect.GoldenAmulet:
                player.SetGold(player.currentGold + 200);
                break;
            case ItemEffect.CommonCherry:
                player.SetCurrentHealth((int)(player.currentPlayerHP * 0.5 + 1), player.maxPlayerHP);
                player.playerDamage += 1;
                break;
            case ItemEffect.DamageCristal:
                player.playerDamage += 10;
                break;
            default:
                break;
        }
    }
}
