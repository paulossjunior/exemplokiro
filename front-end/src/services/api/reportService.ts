import httpClient from './httpClient';
import type { GenerateReportRequest, AccountabilityReportResponse } from '@/types/api';

/**
 * ReportService - API service for report generation and download operations
 * 
 * Provides methods to interact with the back-end Reports API endpoints
 * for generating accountability reports and downloading report files.
 */
export class ReportService {
  private readonly basePath = '/api/reports';

  /**
   * Generate an accountability report for a project
   * 
   * Creates a comprehensive accountability report including project details,
   * transactions, and optionally audit trail information. The report includes
   * integrity verification to ensure data authenticity.
   * 
   * @param projectId - Project ID to generate report for
   * @param data - Report generation parameters
   * @param data.includeAuditTrail - Whether to include audit trail in the report
   * @param data.startDate - Optional start date for filtering transactions (ISO 8601 format)
   * @param data.endDate - Optional end date for filtering transactions (ISO 8601 format)
   * @returns Promise with generated report data
   * @throws ApiError if project not found (404), validation fails (400), or other errors
   */
  async generateAccountabilityReport(
    projectId: string,
    data: GenerateReportRequest
  ): Promise<AccountabilityReportResponse> {
    const response = await httpClient.post<AccountabilityReportResponse>(
      `${this.basePath}/accountability/${projectId}`,
      data
    );
    return response.data;
  }

  /**
   * Download a report file as PDF
   * 
   * Downloads the generated report as a PDF blob. The blob can be used to
   * create a download link or display the PDF in the browser.
   * 
   * @param reportId - Report ID to download
   * @returns Promise with PDF blob
   * @throws ApiError if report not found (404) or other errors
   * 
   * @example
   * ```typescript
   * const blob = await reportService.downloadReport(reportId);
   * const url = URL.createObjectURL(blob);
   * const link = document.createElement('a');
   * link.href = url;
   * link.download = `report-${reportId}.pdf`;
   * link.click();
   * URL.revokeObjectURL(url);
   * ```
   */
  async downloadReport(reportId: string): Promise<Blob> {
    const response = await httpClient.get(`${this.basePath}/${reportId}/download`, {
      responseType: 'blob',
    });
    return response.data;
  }
}

// Export singleton instance
export const reportService = new ReportService();
