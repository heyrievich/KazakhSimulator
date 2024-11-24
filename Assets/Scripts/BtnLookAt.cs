using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // Ссылка на камеру

    private void Start()
    {
        // Если камера не задана, автоматически найти основную камеру
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        LookAt();
    }

    private void LookAt()
    {
        // Получаем направление от объекта к камере
        Vector3 directionToCamera = cameraTransform.position - transform.position;

        // Проецируем это направление на горизонтальную плоскость (убираем Y-компоненту)
        directionToCamera.y = 0;

        // Если длина направления больше нуля, настраиваем вращение объекта
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // Добавляем поворот на 180 градусов вокруг Y-оси, чтобы устранить зеркальность
            transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0);
        }
    }
}
