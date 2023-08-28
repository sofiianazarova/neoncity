using UnityEngine;

[CreateAssetMenu]
public class CharacterTemplate : ScriptableObject
{
    public AnimationData IdleAnimation;
    public AnimationData WalkAnimation;
    public AnimationData RunAnimation;
    public AnimationData JumpAnimation;
}
