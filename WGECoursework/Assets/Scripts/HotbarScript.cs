using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    public int currentlySelected = 0;
    public GameObject selector;

    Vector2 selectorInitialPosition;
    public float selectorWidth;

    // Start is called before the first frame update
    void Start()
    {
        selectorInitialPosition = selector.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the selector according to the currently selected int
        selector.GetComponent<RectTransform>().anchoredPosition = new Vector2(selectorInitialPosition.x + (selectorWidth * currentlySelected), selectorInitialPosition.y);


        // Keycode controls
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentlySelected = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentlySelected = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentlySelected = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentlySelected = 3;

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel < 0) SelectNext();
        if (scrollWheel > 0) SelectPrevious();
    }

    public void SelectNext()
    {
        currentlySelected++;
        if (currentlySelected > 3) currentlySelected = 0;
    }

    public void SelectPrevious()
    {
        currentlySelected--;
        if (currentlySelected < 0) currentlySelected = 3;
    }
}
