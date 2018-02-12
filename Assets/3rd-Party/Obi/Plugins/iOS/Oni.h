/*
 *  Oni.h
 *  Oni
 *
 *  Created by José María Méndez González on 21/9/15.
 *  Copyright (c) 2015 ArK. All rights reserved.
 *
 */

#ifndef Oni_
#define Oni_

#include "Solver.h"
#include "HalfEdgeMesh.h"
#include "ParticleGrid.h"

#if defined(__APPLE__) || defined(ANDROID)
    #define EXPORT __attribute__((visibility("default")))
#else
    #define EXPORT __declspec(dllexport)
#endif

namespace Oni
{
    
    struct ConstraintGroupParameters;
    class ConstraintBatchBase;
    class Collider;
    class Rigidbody;
    class TriangleSkinMap;
    struct SphereShape;
    struct BoxShape;
    struct CapsuleShape;
    struct HeightmapShape;
    struct TriangleMeshShape;
    struct CollisionMaterial;
    struct ProfileInfo;
    
    // New API:
    template<class T>
    struct ObjHandle{
        std::shared_ptr<T> ptr;
        ObjHandle(T* obj):ptr(std::shared_ptr<T>(obj)){}
        std::shared_ptr<T> operator->() const{
            return ptr;
        }
    };
    
    extern "C"
    {
    
        typedef ObjHandle<Collider> ColliderHandle;
        typedef ObjHandle<Shape> ShapeHandle;
        typedef ObjHandle<Rigidbody> RigidbodyHandle;
        typedef ObjHandle<CollisionMaterial> CollisionMaterialHandle;
        
        // Colliders ********************:
        
        EXPORT ColliderHandle* CreateCollider();
        EXPORT void DestroyCollider(ColliderHandle* collider);
        
        EXPORT ShapeHandle* CreateShape(const ShapeType shape);
        EXPORT void DestroyShape(ShapeHandle* shape);
        
        EXPORT RigidbodyHandle* CreateRigidbody();
        EXPORT void DestroyRigidbody(RigidbodyHandle* rigidbody);
        
        EXPORT void UpdateCollider(ColliderHandle* collider, const ColliderAdaptor& adaptor);
        EXPORT void UpdateShape(ShapeHandle* shape, const ShapeAdaptor& adaptor);
        EXPORT void UpdateRigidbody(RigidbodyHandle* rigidbody, const RigidbodyAdaptor& adaptor);
        
        EXPORT void SetColliderShape(ColliderHandle* collider, ShapeHandle* shape);
        EXPORT void SetColliderRigidbody(ColliderHandle* collider, RigidbodyHandle* rigidbody);
        EXPORT void SetColliderMaterial(ColliderHandle* collider, CollisionMaterialHandle* material);
        
        EXPORT void GetRigidbodyVelocity(RigidbodyHandle* rigidbody, RigidbodyVelocityDelta& delta);
        
        // Collision materials *****************:
        
        EXPORT CollisionMaterialHandle* CreateCollisionMaterial();
        EXPORT void UpdateCollisionMaterial(CollisionMaterialHandle* material, const CollisionMaterial& adaptor);
        EXPORT void DestroyCollisionMaterial(CollisionMaterialHandle* collider);
        
        // Solver ********************:
        
		EXPORT Solver* CreateSolver(int max_particles, int max_neighbours);
		EXPORT void DestroySolver(Solver* solver);
        
        EXPORT void AddCollider(Solver* solver,ColliderHandle* collider);
        EXPORT void RemoveCollider(Solver* solver,ColliderHandle* collider);
        
		EXPORT void GetBounds(Solver* solver, Eigen::Vector3f& min, Eigen::Vector3f& max);
        EXPORT int GetParticleGridSize(Solver* solver);
        EXPORT void GetParticleGrid(Solver* solver, ParticleGrid::GridCell* cells);
        
		EXPORT void SetSolverParameters(Solver* solver, const SolverParameters* parameters);
		EXPORT void GetSolverParameters(Solver* solver, SolverParameters* parameters);
        
		EXPORT void AddSimulationTime(Solver* solver, const float step_seconds);
        EXPORT void ResetSimulationTime(Solver* solver);
		EXPORT void UpdateSolver(Solver* solver, const float substep_seconds);
		EXPORT void ApplyPositionInterpolation(Solver* solver,const float substep_seconds);
        EXPORT void UpdateSkeletalAnimation(Solver* solver);
        
		EXPORT void SetConstraintsOrder(Solver* solver, const int* order);
		EXPORT void GetConstraintsOrder(Solver* solver, int* order);
		EXPORT int GetConstraintCount(Solver* solver, const Solver::ConstraintType type);
        EXPORT void GetActiveConstraintIndices(Solver* solver, int* indices, int num, const Solver::ConstraintType type);
        
		EXPORT int SetActiveParticles(Solver* solver, const int* active, int num);
        
		EXPORT int SetParticlePhases(Solver* solver,const int* phases, int num, int dest_offset);
        
		EXPORT int SetParticlePositions(Solver* solver,const float* positions, int num, int dest_offset);
        
        EXPORT int GetParticlePositions(Solver* solver, float* positions, int num, int source_offset);
        
		EXPORT int SetRenderableParticlePositions(Solver* solver,const float* positions, int num, int dest_offset);
        
        EXPORT int GetRenderableParticlePositions(Solver* solver, float* positions, int num, int source_offset);
        
		EXPORT int SetParticleInverseMasses(Solver* solver, const float* inv_masses,int num, int dest_offset);
        
		EXPORT int SetParticleSolidRadii(Solver* solver, const float* radii,int num, int dest_offset);
        
		EXPORT int SetParticleVelocities(Solver* solver,const float* velocities, int num, int dest_offset);
        
        EXPORT void AddParticleExternalForces(Solver* solver, const Vector4fUnaligned* forces, int* indices, int num);
        
        EXPORT void AddParticleExternalForce(Solver* solver, const Vector4fUnaligned& force, int* indices, int num);
        
        EXPORT int GetParticleVelocities(Solver* solver, float* velocities, int num, int source_offset);
        
        EXPORT int GetDeformableTriangleCount(Solver* solver);
        
        EXPORT void SetDeformableTriangles(Solver* solver, const int* indices, int num, int dest_offset);
        
        EXPORT int RemoveDeformableTriangles(Solver* solver, int num, int source_offset);
        
        EXPORT int GetParticleVorticities(Solver* solver, float* vorticities, int num, int source_offset);
        
        EXPORT int GetParticleDensities(Solver* solver, float* densities, int num, int dest_offset);
        
		EXPORT void SetConstraintGroupParameters(Solver* solver, const Solver::ConstraintType type, const ConstraintGroupParameters* parameters);
        
		EXPORT void GetConstraintGroupParameters(Solver* solver, const Solver::ConstraintType type, ConstraintGroupParameters* parameters);
        
		EXPORT void SetCollisionMaterials(Solver* solver, const CollisionMaterialHandle** materials, int* indices, int num);
        
        EXPORT int SetRestPositions(Solver* solver, const float* positions, int num, int dest_offset);
        
		EXPORT void SetFluidMaterials(Solver* solver, FluidMaterial* materials, int num, int dest_offset);
        
		EXPORT int SetFluidMaterialIndices(Solver* solver, const int* indices, int num, int dest_offset);
        
        // Meshes ********************:
        
        EXPORT Mesh* CreateDeformableMesh(Solver* solver,
                                          HalfEdgeMesh* half_edge,
                                          ConstraintBatchBase* skin_batch,
                                          float world_to_local[16],
                                          const int* particle_indices,
                                          int vertex_capacity,
                                          int vertex_count);
        
        EXPORT void DestroyDeformableMesh(Solver* solver,Mesh* mesh);
        
        EXPORT bool TearDeformableMeshAtVertex(Mesh* mesh,int vertex_index,
                                                          const Eigen::Vector3f* plane_point,
                                                          const Eigen::Vector3f* plane_normal,
                                                          int* updated_edges,
                                                          int& num_edges);
        
        EXPORT void SetDeformableMeshTBNUpdate(Mesh* mesh, const Mesh::NormalUpdate normal_update, bool skin_tangents);
        
        EXPORT void SetDeformableMeshTransform(Mesh* mesh,float world_to_local[16]);
        
        EXPORT void SetDeformableMeshSkinMap(Mesh* mesh, Mesh* source_mesh, TriangleSkinMap* map);
        
        EXPORT void SetDeformableMeshParticleIndices(Mesh* mesh,const int* indices);
        
        EXPORT void SetDeformableMeshData(Mesh* mesh,int* triangles,
                                                     Eigen::Vector3f* vertices,
                                                     Eigen::Vector3f* normals,
                                                     Vector4fUnaligned* tangents,
                                                     Vector4fUnaligned* colors,
                                                     Vector2fUnaligned* uv1,
                                                     Vector2fUnaligned* uv2,
                                                     Vector2fUnaligned* uv3,
                                                     Vector2fUnaligned* uv4);
        
        EXPORT void SetDeformableMeshAnimationData(Mesh* mesh,
                                                   float* bind_poses,
                                                   Mesh::BoneWeight* bone_weights,
                                                   int num_bones);
        
        EXPORT void SetDeformableMeshBoneTransforms(Mesh* mesh,float* bone_transforms);
        
        EXPORT void ForceDeformableMeshSkeletalSkinning(Mesh* mesh);
        
        // Batches ********************:
        
        EXPORT ConstraintBatchBase* CreateBatch(const Solver::ConstraintType type, bool cooked);
        
        EXPORT void DestroyBatch(ConstraintBatchBase* batch);
        
        EXPORT void AddBatch(Solver* solver, ConstraintBatchBase* batch, bool shares_particles);
        
        EXPORT void RemoveBatch(Solver* solver, ConstraintBatchBase* batch);
        
        EXPORT void EnableBatch(ConstraintBatchBase* batch, bool enabled);
        
        EXPORT int GetBatchConstraintCount(ConstraintBatchBase* batch);
        
        EXPORT int GetBatchConstraintForces(ConstraintBatchBase* batch, float* forces, int num, int source_offset);
        
        EXPORT int GetBatchPhaseCount(ConstraintBatchBase* batch);
        
        EXPORT void SetBatchPhaseSizes(ConstraintBatchBase* batch, int* phase_sizes, int num);
        
        EXPORT void GetBatchPhaseSizes(ConstraintBatchBase* batch, int* phase_sizes);
        
        EXPORT bool CookBatch(ConstraintBatchBase* batch);
        
        // Constraints ********************:
        
        EXPORT int SetActiveConstraints(ConstraintBatchBase* batch, const int* active, int num);
        
		EXPORT void SetDistanceConstraints(ConstraintBatchBase* batch,
                                     const int* indices,
                                     const float* restLengths,
                                     const float* stiffnesses,
                                     int num);
        
        EXPORT void GetDistanceConstraints(ConstraintBatchBase* batch,
                                           int* indices,
                                           float* rest_lengths,
                                           float* stiffnesses);
        
		EXPORT void SetBendingConstraints(ConstraintBatchBase* batch,
                                          const int* indices,
                                          const float* rest_bends,
                                          const float* bending_stiffnesses,
                                          int num);
        
        EXPORT void GetBendingConstraints(ConstraintBatchBase* batch,
                                          int* indices,
                                          float* rest_bends,
                                          float* bending_stiffnesses);
        
		EXPORT void SetSkinConstraints(ConstraintBatchBase* batch,
                                       const int* indices,
                                       const Vector4fUnaligned* skin_points,
                                       const Vector4fUnaligned* skin_normals,
                                       const float* radii_backstops,
                                       const float* stiffnesses,
                                       int num);
        
        EXPORT void GetSkinConstraints(ConstraintBatchBase* batch,
                                       int* indices,
                                       Vector4fUnaligned* skin_points,
                                       Vector4fUnaligned* skin_normals,
                                       float* radii_backstops,
                                       float* stiffnesses);
        
		EXPORT void SetAerodynamicConstraints(ConstraintBatchBase* batch,
                                              const int* triangle_indices,
                                              const float* aerodynamic_coeffs,
                                              int num);
        
		EXPORT  void SetVolumeConstraints(ConstraintBatchBase* batch,
                                          const int* triangle_indices,
                                          const int* first_triangle,
                                          const int* num_triangles,
                                          const float* rest_volumes,
                                          const float* pressure_stiffnesses,
                                          int num);
        
        EXPORT  void SetShapeMatchingConstraints(ConstraintBatchBase* batch,
                                                 const int* shape_indices,
                                                 const int* first_index,
                                                 const int* num_indices,
                                                 const float* shape_stiffness,
                                                 int num);
        
        EXPORT void SetTetherConstraints(ConstraintBatchBase* batch,
                                         const int* indices,
                                         const float* max_lenght_scales,
                                         const float* stiffnesses,
                                         int num);
        
        EXPORT void GetTetherConstraints(ConstraintBatchBase* batch,
                                         int* indices,
                                         float* max_lenght_scales,
                                         float* stiffnesses);
        
        EXPORT void SetPinConstraints(ConstraintBatchBase* batch,
                                      const int* indices,
                                      const Vector4fUnaligned* pin_offsets,
                                      const ColliderHandle** colliders,
                                      const float* stiffnesses,
                                      int num);
        
        EXPORT void SetStitchConstraints(ConstraintBatchBase* batch,
                                         const int* indices,
                                         const float* stiffnesses,
                                         int num);
        
        // Collision data ********************:
        
        EXPORT void GetCollisionContacts(Solver* solver,Contact* contacts, int num);
        
        // Diffuse particles ********************:
        
        EXPORT void ClearDiffuseParticles(Solver* solver);
        
        EXPORT int SetDiffuseParticles(Solver* solver, const Vector4fUnaligned* positions, int num);
        
        EXPORT int GetDiffuseParticleVelocities(Solver* solver, Vector4fUnaligned* velocities, int num, int source_offset);
        
		EXPORT void SetDiffuseParticleNeighbourCounts(Solver* solver,int* neighbour_counts);
        
        // Skin maps ********************:
        
        EXPORT TriangleSkinMap* CreateTriangleSkinMap();
        
        EXPORT void DestroyTriangleSkinMap(TriangleSkinMap* map);
        
        EXPORT void Bind(TriangleSkinMap* map, Mesh* source, Mesh* target,
                         const unsigned int* source_master_flags,
                         const unsigned int* target_slave_flags);
        
        EXPORT int GetSkinnedVertexCount(TriangleSkinMap* map);
        
        EXPORT void GetSkinInfo(TriangleSkinMap* map,
                                int* skin_indices,
                                int* source_tri_indices,
                                Eigen::Vector3f* bary_pos,
                                Eigen::Vector3f* bary_nrm,
                                Eigen::Vector3f* bary_tan);
        
       EXPORT  void SetSkinInfo(TriangleSkinMap* map,
                                const int* skin_indices,
                                const int* source_tri_indices,
                                const Eigen::Vector3f* bary_pos,
                                const Eigen::Vector3f* bary_nrm,
                                const Eigen::Vector3f* bary_tan,
                                int num);

        // Tasks ********************:
        
        EXPORT void WaitForAllTasks();
        
        EXPORT void ClearTasks();
        
        // Profiling ****************:
        
        EXPORT int GetMaxSystemConcurrency();
        
        EXPORT void SignalFrameStart();
        
        EXPORT double SignalFrameEnd();
        
        EXPORT void EnableProfiler(bool enabled);
        
        EXPORT int GetProfilingInfoCount();
        
        EXPORT void GetProfilingInfo(ProfileInfo* info, int count);
    }
    
}

#endif
