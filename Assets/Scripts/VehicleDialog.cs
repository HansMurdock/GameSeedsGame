using UnityEngine;

public class VehicleDialog : MonoBehaviour
{
    [TextArea] public string line = "My hands just know which button it is... how?";
    public float duration = 4f;
    public float ignoreFirstSeconds = 1f; // abaikan trigger di detik-detik awal (anti nyala pas load)

    private BaseVehicle vehicle;
    private bool shown = false;
    private float timer = 0f;

    void Start() { vehicle = GetComponent<BaseVehicle>(); }

    void Update()
    {
        timer += Time.deltaTime;
        if (shown || vehicle == null) return;
        if (timer < ignoreFirstSeconds) return; // skip detik awal

        if (vehicle.IsOccupied)
        {
            shown = true;
            GameDialog.Show(line, duration);
        }
    }
}