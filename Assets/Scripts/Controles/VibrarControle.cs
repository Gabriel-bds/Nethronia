using UnityEngine.InputSystem;
using System.Threading.Tasks;

public static class VibrarControle
{
    public static async void Vibrar(float esquerda, float direita, float tempo)
    {
        if (Gamepad.current == null) return;

        Gamepad.current.SetMotorSpeeds(esquerda, direita);

        int tempoMs = (int)(tempo * 1000);

        await Task.Delay(tempoMs);

        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}