using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obi{

[RequireComponent(typeof(ObiSolver))]
public class ObiParticleGridDebugger : MonoBehaviour {

	ObiSolver solver;
	Oni.GridCell[] cells;

	void OnEnable () {
		solver = GetComponent<ObiSolver>();
		solver.OnFrameEnd += Solver_OnFrameEnd;
	}

	void OnDisable () {
		solver.OnFrameEnd -= Solver_OnFrameEnd;
	}

	void Solver_OnFrameEnd (object sender, System.EventArgs e)
	{
		int cellCount = Oni.GetParticleGridSize(solver.OniSolver);
		cells = new Oni.GridCell[cellCount];
		Oni.GetParticleGrid(solver.OniSolver, cells);
	}

	void OnDrawGizmos(){

		if (cells == null) return;

		foreach (Oni.GridCell cell in cells){

			Gizmos.color = (cell.count > 0) ? Color.yellow:Color.red;
			Gizmos.DrawWireCube(cell.center,cell.size);
			
		}
	}
	
}
}
