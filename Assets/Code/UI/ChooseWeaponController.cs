using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseWeaponController : MonoBehaviour {

    [SerializeField] private List<Image> WeaponImages;

    private List<WeaponData> AllWeapons;
    private List<WeaponData> SelectedWeapons;
    private bool WasWeaponSelected = false;

    void Start() {
        AllWeapons = LevelManager.Instance.GetWeaponDatas().WeaponDatas;
        SelectedWeapons = new List<WeaponData>();
        while (SelectedWeapons.Count < 3) {
            WeaponData randomWeapon = AllWeapons[Random.Range(0, AllWeapons.Count)];
            if (!SelectedWeapons.Contains(randomWeapon)) {
                SelectedWeapons.Add(randomWeapon);
            }
        }

        for (int i = 0; i < WeaponImages.Count; i++) {
            WeaponImages[i].sprite = SelectedWeapons[i].weaponIcon;
        }
    }

    public void Select(int index) {
        if (WasWeaponSelected) {
            return;
        }

        for (int i = 0; i < AllWeapons.Count; i++) {
            if (SelectedWeapons[index] == AllWeapons[i]) {
                LevelManager.Instance.SetWeaponIndex(i);
                break;
            }
        }

        //temp
        OnChanceScene();
    }

    public void OnChanceScene() {
        SceneManager.LoadScene("Game");
    }
}
