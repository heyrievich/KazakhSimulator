using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // ������ �� ������

    private void Start()
    {
        // ���� ������ �� ������, ������������� ����� �������� ������
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
        // �������� ����������� �� ������� � ������
        Vector3 directionToCamera = cameraTransform.position - transform.position;

        // ���������� ��� ����������� �� �������������� ��������� (������� Y-����������)
        directionToCamera.y = 0;

        // ���� ����� ����������� ������ ����, ����������� �������� �������
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // ��������� ������� �� 180 �������� ������ Y-���, ����� ��������� ������������
            transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0);
        }
    }
}
