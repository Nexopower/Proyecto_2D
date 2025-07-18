using UnityEngine;

public static class EnemyUtils
{
    public static bool DetectWall(Transform origin, float distance, LayerMask whatIsWall, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin.position, direction, distance, whatIsWall);
        return hit.collider != null;
    }

    public static bool DetectEdge(Transform origin, float distance, LayerMask whatIsGround, Vector2 direction)
    {
        Vector2 raycastPosition = origin.position + (Vector3)direction * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, distance, whatIsGround);
        return hit.collider == null;
    }

    public static bool IsPlayerInRange(Transform self, Transform player, float range)
    {
        return Vector2.Distance(self.position, player.position) <= range;
    }

    public static bool IsPlayerInFront(Transform self, Transform player)
    {
        Vector2 dirToPlayer = player.position - self.position;
        return (self.localScale.x > 0 && dirToPlayer.x > 0) || (self.localScale.x < 0 && dirToPlayer.x < 0);
    }

    public static bool IsPlayerBehind(Transform self, Transform player)
    {
        Vector2 dirToPlayer = player.position - self.position;
        return (self.localScale.x > 0 && dirToPlayer.x < 0) || (self.localScale.x < 0 && dirToPlayer.x > 0);
    }

    public static bool IsPlayerLookingAtEnemy(Transform self, Transform player)
    {
        Vector2 dirToEnemy = self.position - player.position;
        float playerFacing = player.localScale.x;
        return (playerFacing > 0 && dirToEnemy.x > 0) || (playerFacing < 0 && dirToEnemy.x < 0);
    }

    public static void Flip(Transform obj)
    {
        Vector3 scale = obj.localScale;
        scale.x *= -1;
        obj.localScale = scale;
    }

    public static Vector2 GetDirectionToPlayer(Transform self, Transform player)
    {
        return (player.position - self.position).normalized;
    }

    public static bool DetectEnemy(Transform origin, float distance, LayerMask enemyLayer, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin.position, direction, distance, enemyLayer);
        return hit.collider != null && hit.collider.transform != origin;
    }
}
