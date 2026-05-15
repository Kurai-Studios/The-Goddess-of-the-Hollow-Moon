using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    public Image openTab;
    public Image closeTab;

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].sprite = closeTab.sprite;
        }

        pages[tabNo].SetActive(true);
        tabImages[tabNo].sprite = openTab.sprite;
    }
}
