using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace SBaier.Master
{
	public class HullInfoTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private const int _pointerId = -2;
		private HullInfo _hullInfo;
		private PlanetFace _planetFace;

		[SerializeField]
		private Vector2 _offset = new Vector2(10, 0);

		[Inject]
		public void Construct(HullInfo hullInfo, PlanetFace planetFace)
		{
			_hullInfo = hullInfo;
			_planetFace = planetFace;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.pointerId != _pointerId)
				return;
			Vector3 localPoint = _planetFace.transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
			int index = _planetFace.GetNearestTo(localPoint);
			EvaluationPointData data = _planetFace.Data.EvaluationPoints[index];
			_hullInfo.Show(eventData.position + _offset, data, _planetFace.PlanetData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_hullInfo.Hide();
		}
	}
}