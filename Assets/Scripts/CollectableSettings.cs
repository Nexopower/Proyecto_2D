using UnityEngine;

[CreateAssetMenu(fileName = "CollectableSettings", menuName = "Game/Collectable Settings")]
public class CollectableSettings : ScriptableObject
{
    [Header("Score Values")]
    public int coinValue = 10;
    public int gemValue = 20;
    
    [Header("Audio Clips")]
    public AudioClip coinSound;
    public AudioClip gemSound;
    public AudioClip keySound;
    public AudioClip heartSound;
    
    [Header("Particle Effects")]
    public ParticleSystem coinEffect;
    public ParticleSystem gemEffect;
    public ParticleSystem keyEffect;
    public ParticleSystem heartEffect;
    
    [Header("UI Settings")]
    public Sprite coinIcon;
    public Sprite gemIcon;
    public Sprite keyIcon;
    public Sprite heartIcon;
    
    // Método para obtener el valor según el tipo
    public int GetScoreValue(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                return coinValue;
            case CollectableType.Gem:
                return gemValue;
            default:
                return 0;
        }
    }
    
    // Método para obtener el sonido según el tipo
    public AudioClip GetSound(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                return coinSound;
            case CollectableType.Gem:
                return gemSound;
            case CollectableType.Key:
                return keySound;
            case CollectableType.Heart:
                return heartSound;
            default:
                return null;
        }
    }
    
    // Método para obtener el efecto según el tipo
    public ParticleSystem GetEffect(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                return coinEffect;
            case CollectableType.Gem:
                return gemEffect;
            case CollectableType.Key:
                return keyEffect;
            case CollectableType.Heart:
                return heartEffect;
            default:
                return null;
        }
    }
}