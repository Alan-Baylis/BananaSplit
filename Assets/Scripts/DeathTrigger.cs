using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public delegate void DeathTriggerEvent();
    public static DeathTriggerEvent OnDeathTrigger;

    private void OnTriggerEnter(Collider col)
    {
        var split = col.gameObject.GetComponent<Splitable>();

        // We only want to destroy fruits/splittables
        if (split != null)
        {
            // If the fruit has yet to be split,
            // it counts as a dropped fruit
            bool fruitDropped = split.FireSplitEvent;

            if (OnDeathTrigger != null && fruitDropped)
                OnDeathTrigger();

            // Destroy the fruit anyway, dropped or not
            Destroy(col.gameObject);
        }
    }
}
