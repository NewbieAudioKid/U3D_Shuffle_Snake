// ================================================================================
// TL;DR:
// 临时输入管理器，用于测试贪吃蛇移动（使用键盘方向键）
// 后续会替换为触摸拖拽输入
//
// 目标：
// - 提供键盘输入控制（WASD 或 方向键）
// - 将输入转换为方向向量传递给 SnakeController
//
// 非目标：
// - 这是临时测试代码，最终会被触摸输入替代
// ================================================================================
using UnityEngine;

public class TemporaryKeyboardInput : MonoBehaviour
{
    void Update()
    {
        if (SnakeController.Instance == null) return;

        // 获取键盘输入
        Vector2 inputDirection = Vector2.zero;

        // WASD 或 方向键
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            inputDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            inputDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputDirection += Vector2.right;

        // 传递给蛇控制器
        if (inputDirection.magnitude > 0.1f)
        {
            SnakeController.Instance.SetDirection(inputDirection);
        }
    }
}

