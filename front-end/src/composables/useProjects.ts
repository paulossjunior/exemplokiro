import { ref, computed } from 'vue';
import { projectService } from '@/services/api/projectService';
import { useApi } from './useApi';
import type { 
  Project, 
  CreateProjectRequest, 
  UpdateProjectRequest,
  ProjectStatus 
} from '@/types/api';

/**
 * useProjects - Composable for project management operations
 * 
 * Provides reactive state and methods for managing projects including
 * fetching, creating, updating, and status management.
 * 
 * @example
 * ```typescript
 * const { projects, currentProject, loading, fetchProjects, createProject } = useProjects();
 * 
 * // Fetch all projects
 * await fetchProjects();
 * 
 * // Create a new project
 * await createProject({
 *   name: 'New Project',
 *   description: 'Project description',
 *   // ... other fields
 * });
 * ```
 */
export function useProjects() {
  // State
  const projects = ref<Project[]>([]);
  const currentProject = ref<Project | null>(null);

  // Fetch projects with pagination and filtering
  const { 
    loading: loadingProjects, 
    error: projectsError,
    execute: executeFetchProjects 
  } = useApi(
    async (params?: { status?: string; pageNumber?: number; pageSize?: number }) => {
      const response = await projectService.getProjects(params);
      projects.value = response.items;
      return response;
    }
  );

  // Fetch single project
  const { 
    loading: loadingProject, 
    error: projectError,
    execute: executeFetchProject 
  } = useApi(
    async (id: string) => {
      const project = await projectService.getProject(id);
      currentProject.value = project;
      return project;
    }
  );

  // Create project
  const { 
    loading: creatingProject, 
    error: createError,
    execute: executeCreateProject 
  } = useApi(
    async (data: CreateProjectRequest) => {
      const project = await projectService.createProject(data);
      projects.value.push(project);
      return project;
    }
  );

  // Update project
  const { 
    loading: updatingProject, 
    error: updateError,
    execute: executeUpdateProject 
  } = useApi(
    async (id: string, data: UpdateProjectRequest) => {
      const project = await projectService.updateProject(id, data);
      
      // Update in projects array
      const index = projects.value.findIndex((p) => p.id === id);
      if (index !== -1) {
        projects.value[index] = project;
      }
      
      // Update current project if it's the same
      if (currentProject.value?.id === id) {
        currentProject.value = project;
      }
      
      return project;
    }
  );

  // Update project status
  const { 
    loading: updatingStatus, 
    error: statusError,
    execute: executeUpdateStatus 
  } = useApi(
    async (id: string, status: ProjectStatus) => {
      const project = await projectService.updateProjectStatus(id, { status });
      
      // Update in projects array
      const index = projects.value.findIndex((p) => p.id === id);
      if (index !== -1) {
        projects.value[index] = project;
      }
      
      // Update current project if it's the same
      if (currentProject.value?.id === id) {
        currentProject.value = project;
      }
      
      return project;
    }
  );

  // Computed loading state - true if any operation is in progress
  const loading = computed(
    () => 
      loadingProjects.value || 
      loadingProject.value || 
      creatingProject.value || 
      updatingProject.value || 
      updatingStatus.value
  );

  // Computed error state - returns the first error found
  const error = computed(
    () => 
      projectsError.value || 
      projectError.value || 
      createError.value || 
      updateError.value || 
      statusError.value
  );

  // Public methods
  const fetchProjects = (params?: { status?: string; pageNumber?: number; pageSize?: number }) => {
    return executeFetchProjects(params);
  };

  const fetchProject = (id: string) => {
    return executeFetchProject(id);
  };

  const createProject = (data: CreateProjectRequest) => {
    return executeCreateProject(data);
  };

  const updateProject = (id: string, data: UpdateProjectRequest) => {
    return executeUpdateProject(id, data);
  };

  const updateStatus = (id: string, status: ProjectStatus) => {
    return executeUpdateStatus(id, status);
  };

  return {
    // State
    projects,
    currentProject,
    
    // Computed
    loading,
    error,
    
    // Methods
    fetchProjects,
    fetchProject,
    createProject,
    updateProject,
    updateStatus,
  };
}
