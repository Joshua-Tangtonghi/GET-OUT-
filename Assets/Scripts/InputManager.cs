using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        // Détection des touches du clavier (via JoyKey)
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Touche "1"
        {
            OnButton1Pressed();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Touche "2"
        {
            OnButton2Pressed();
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) // Espace
        {
            OnSpacePressed();
        }
        
        // Détection du touchscreen (pour les questions/interactions)
        HandleTouchInput();
    }
    
    void HandleTouchInput()
    {
        // Gestion multi-touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch.position);
                    break;
                    
                case TouchPhase.Moved:
                    OnTouchMoved(touch.position);
                    break;
                    
                case TouchPhase.Ended:
                    OnTouchEnded(touch.position);
                    break;
            }
        }
        
        // Alternative pour tester dans l'éditeur Unity (souris)
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            OnTouchBegan(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnded(Input.mousePosition);
        }
        #endif
    }
    
    // Méthodes pour les actions des boutons
    void OnButton1Pressed()
    {
        Debug.Log("Bouton 1 pressé via JoyKey!");
        // Votre logique ici
    }
    
    void OnButton2Pressed()
    {
        Debug.Log("Bouton 2 pressé via JoyKey!");
        // Votre logique ici
    }
    
    void OnSpacePressed()
    {
        Debug.Log("Espace pressé via JoyKey!");
        // Votre logique ici
    }
    
    // Méthodes pour les actions tactiles
    void OnTouchBegan(Vector2 position)
    {
        Debug.Log($"Touch commencé à: {position}");
        
        // Exemple: détecter si on touche un UI ou un objet
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Objet touché: {hit.collider.gameObject.name}");
            // Traiter l'interaction avec l'objet
        }
    }
    
    void OnTouchMoved(Vector2 position)
    {
        // Gérer le déplacement du doigt
    }
    
    void OnTouchEnded(Vector2 position)
    {
        Debug.Log($"Touch terminé à: {position}");
        // Gérer la fin du touch
    }
}