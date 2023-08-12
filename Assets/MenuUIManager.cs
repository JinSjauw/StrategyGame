using CustomInput;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _mainMenu;
    [SerializeField] private Transform _inventory;
    [SerializeField] private Transform _market;
    [SerializeField] private Transform _shopWindow;
    
    private void Awake()
    {
        //Enable MENU Actions
        _inputReader.EnableMainMenuInput();
    }
    
    public void OpenInventory()
    {
        _inputReader.EnableMainMenuInput();
        _mainMenu.gameObject.SetActive(false);
        _inventory.gameObject.SetActive(true);
    }
    
    public void CloseInventory()
    {
        _inputReader.DisableMainMenuInput();
        _inventory.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
    }

    public void OpenMarket()
    {
        _mainMenu.gameObject.SetActive(false);
        _market.gameObject.SetActive(true);
    }

    public void CloseMarket()
    {
        _market.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
    }
    
    public void OpenShop()
    {
        _market.gameObject.SetActive(false);
        _shopWindow.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        _shopWindow.gameObject.SetActive(false);
        _market.gameObject.SetActive(true);
    }
}
