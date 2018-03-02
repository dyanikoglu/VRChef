using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Obi;
using System;

[RequireComponent(typeof(ObiSolver))]
public class FluidCollisionDetection : MonoBehaviour
{
    int count = 0;
    List<int> collidedParticles;

    int numberOfNeededFluidParticles;
    CanBoil canBoilScript;
    CanFry canFryScript;

    ObiSolver solver;

    Obi.ObiSolver.ObiCollisionEventArgs collisionEvent;

    bool collidedWithBowl = false;

    void Awake()
    {
        solver = GetComponent<Obi.ObiSolver>();
        collidedParticles = new List<int>();
        numberOfNeededFluidParticles = GetComponent<ObiEmitter>().NumParticles;
    }

    void OnEnable()
    {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable()
    {
        solver.OnCollision -= Solver_OnCollision;
    }

    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {
        foreach (Oni.Contact contact in e.contacts)
        {
            // this one is an actual collision:
            if (contact.distance < 0.01)
            {
                Component collider;
                if (ObiCollider.idToCollider.TryGetValue(contact.other, out collider))
                {

                    if (collider.gameObject.GetComponent<CanBoil>())
                    {
                        canBoilScript = collider.gameObject.GetComponent<CanBoil>();
                        int index = Int32.Parse(contact.particle.ToString());
                        if (!collidedParticles.Contains(index))
                        {
                            collidedParticles.Add(index);
                        }
                    }

                    else if (collider.gameObject.GetComponentInParent<CanFry>())
                    {
                        canFryScript = collider.gameObject.GetComponentInParent<CanFry>();
                        int index = Int32.Parse(contact.particle.ToString());
                        if (!collidedParticles.Contains(index))
                        {
                            collidedParticles.Add(index);
                        }
                    }

                    if (!collidedWithBowl && collider.transform.parent)
                    {
                        if (!collidedWithBowl && collider.transform.parent.transform.parent)
                        {
                            if (!collidedWithBowl && collider.transform.parent.transform.parent.gameObject.GetComponent<CanMixedIn>() != null)
                            {
                                collidedWithBowl = true;
                                List<Obi.ObiEmitter> emitters = collider.transform.parent.transform.parent.gameObject.GetComponent<CanMixedIn>().emitters;
                                emitters.Add(gameObject.GetComponent<ObiEmitter>());
                            }
                        }
                    }
                    
                }
            }
        }
    }

    private void Update()
    {
        if(collidedParticles.Count >= numberOfNeededFluidParticles * 0.1f)
        {
            if (canBoilScript)
            {
                canBoilScript.SetHasFluid(true);
                OnDisable();
            }
            if (canFryScript)
            {
                canFryScript.SetHasFluid(true);
                OnDisable();
            }
        }
    }

}