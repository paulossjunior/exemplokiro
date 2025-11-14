import httpClient from './httpClient';
import type {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  UpdateProjectStatusRequest,
  PaginatedResponse,
} from '@/types/api';

/**
 * ProjectService - API service for project management operations
 * 
 * Provides methods to interact with the back-end Projects API endpoints
 * including CRUD operations and status management.
 */
export class ProjectService {
  private readonly basePath = '/api/projects';

  /**
   * Get paginated list of projects with optional filtering
   * 
   * @param params - Query parameters for filtering and pagination
   * @param params.status - Filter by project status (NotStarted, InProgress, Completed, Cancelled)
   * @param params.pageNumber - Page number for pagination (default: 1)
   * @param params.pageSize - Number of items per page (default: 10)
   * @returns Promise with paginated project list
   */
  async getProjects(params?: {
    status?: string;
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<Project>> {
    const response = await httpClient.get<PaginatedResponse<Project>>(this.basePath, { params });
    return response.data;
  }

  /**
   * Get a single project by ID
   * 
   * @param id - Project ID
   * @returns Promise with project details
   * @throws ApiError if project not found (404) or other errors
   */
  async getProject(id: string): Promise<Project> {
    const response = await httpClient.get<Project>(`${this.basePath}/${id}`);
    return response.data;
  }

  /**
   * Create a new project
   * 
   * @param data - Project creation data
   * @returns Promise with created project
   * @throws ApiError if validation fails (400) or other errors
   */
  async createProject(data: CreateProjectRequest): Promise<Project> {
    const response = await httpClient.post<Project>(this.basePath, data);
    return response.data;
  }

  /**
   * Update an existing project
   * 
   * @param id - Project ID
   * @param data - Project update data
   * @returns Promise with updated project
   * @throws ApiError if project not found (404), validation fails (400), or other errors
   */
  async updateProject(id: string, data: UpdateProjectRequest): Promise<Project> {
    const response = await httpClient.put<Project>(`${this.basePath}/${id}`, data);
    return response.data;
  }

  /**
   * Update project status
   * 
   * @param id - Project ID
   * @param data - Status update data
   * @returns Promise with updated project
   * @throws ApiError if project not found (404), invalid status (400), or other errors
   */
  async updateProjectStatus(id: string, data: UpdateProjectStatusRequest): Promise<Project> {
    const response = await httpClient.put<Project>(`${this.basePath}/${id}/status`, data);
    return response.data;
  }
}

// Export singleton instance
export const projectService = new ProjectService();
