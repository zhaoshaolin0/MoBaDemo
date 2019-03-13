﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCursorChanged : MonoBehaviour {

    private CharacterMono characterMono;
    private Projector skillCircleInflence;
    private OutLinePostEffect outLinePostEffect;

    public Projector skillCircleInflencePrefabs;


    public void Init() {
        characterMono = GameObject.FindWithTag("Player").GetComponent<CharacterMono>();
        outLinePostEffect = Camera.main.GetComponent<OutLinePostEffect>();
    }

    // Update is called once per frame
    void Update () {
        if (characterMono == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        RaycastHit hit2;
        MouseIconManager.Instace.Recovery();
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag("Enermy")) {
                MouseIconManager.Instace.ChangeMouseIcon(MouseIconManager.MouseState.Attack);

                // 为目标单位添加一个泛光描边
                if (outLinePostEffect.TargetObject != hit.collider.gameObject) {                    
                    outLinePostEffect.TargetObject = hit.collider.gameObject;
                    outLinePostEffect.outLineColor = Color.red;
                }
            } else {
                if (outLinePostEffect.isActiveAndEnabled) {
                    outLinePostEffect.ClearRenderTarget();
                    outLinePostEffect.TargetObject = null;
                }
            }
        }
        if (characterMono.isPrepareUseSkill) {
            MouseIconManager.Instace.ChangeMouseIcon(MouseIconManager.MouseState.Spell);

            ActiveSkill activeSkill = characterMono.prepareSkill;
            if (activeSkill.SkillInfluenceRadius > 0 && !activeSkill.IsMustDesignation) {
                if (Physics.Raycast(ray, out hit2, 100,layerMask:1<<11)) {
                    Vector3 position = hit2.point;
                    //Debug.Log(position);
                    position.y = 3f;
                    if (skillCircleInflence == null) {
                        skillCircleInflence = GameObject.Instantiate<Projector>(skillCircleInflencePrefabs, position, skillCircleInflencePrefabs.transform.rotation);
                    } else {
                        skillCircleInflence.gameObject.SetActive(true);
                        skillCircleInflence.transform.position = position;
                    }
                    skillCircleInflence.orthographicSize = activeSkill.SkillInfluenceRadius;

                }
            }
        } else {
            if (skillCircleInflence != null) {
                skillCircleInflence.gameObject.SetActive(false);
            }
        }

        // 处理用户拾取物品后鼠标图标的改变
        RaycastHit hit3;
        if (characterMono.IsPickUpItem) {
            MouseIconManager.Instace.ChangeMouseIcon(MouseIconManager.MouseState.PickUpItem);
            if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit3) && !EventSystem.current.IsPointerOverGameObject()) {
                characterMono.IsPickUpItem = false;
            }

            // 鼠标右键或ESC键取消拾取
            if ((Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape)) && !EventSystem.current.IsPointerOverGameObject()) {
                characterMono.IsPickUpItem = false;
            }
        }

    }
}
