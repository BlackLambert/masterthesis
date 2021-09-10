using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace SBaier.Master
{
	public class HullInfoTrigger : MonoBehaviour, IPointerDownHandler
	{
		private const int _pointerId = -2;
		private const int _mouseButtonID = 1;
		private HullInfo _hullInfo;
		private PlanetFace _planetFace;
		private bool _shown = false;
		private float _formerTimeScale = 1;

		[SerializeField]
		private Vector2 _offset = new Vector2(10, 0);

		[Inject]
		public void Construct(HullInfo hullInfo, PlanetFace planetFace)
		{
			_hullInfo = hullInfo;
			_planetFace = planetFace;
		}

		protected virtual void Update()
		{
			if (_shown && IsPointerUp())
				HideView();
		}

		private void HideView()
		{
			_hullInfo.Hide();
			Time.timeScale = _formerTimeScale;
			_shown = false;
		}

		private bool IsPointerUp()
		{
			return Input.GetMouseButtonUp(_mouseButtonID);

		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.pointerId != _pointerId)
				return;
			ShowView(eventData);
		}

		private void ShowView(PointerEventData eventData)
		{
			Vector3 localPoint = _planetFace.transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
			int index = _planetFace.GetNearestTo(localPoint);
			EvaluationPointData data = _planetFace.Data.EvaluationPoints[index];
			_hullInfo.Show(eventData.position + _offset, data, _planetFace.PlanetData);
			_formerTimeScale = Time.timeScale;
			Time.timeScale = 0;
			_shown = true;
		}
	}
}