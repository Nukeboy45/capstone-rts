using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    // Update is called once per frame
    void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 customCursorPosition = Input.mousePosition;
        transform.position = customCursorPosition+new Vector3(25 * gameObject.transform.localScale.x,-25 * gameObject.transform.localScale.y, 0);
    }
}
