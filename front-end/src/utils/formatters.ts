/**
 * Utility functions for formatting data
 */

/**
 * Formats a number as Brazilian Real currency
 * @param value - The numeric value to format
 * @returns Formatted currency string (e.g., "R$ 1.500,50")
 * @example
 * formatCurrency(1500.50) // Returns "R$ 1.500,50"
 */
export function formatCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

/**
 * Formats an ISO 8601 date string to Brazilian date/time format
 * @param isoDate - ISO 8601 date string
 * @returns Formatted date/time string (e.g., "13/11/2025 - 14:30:00")
 * @example
 * formatDateTime('2025-11-13T14:30:00Z') // Returns "13/11/2025 - 14:30:00"
 */
export function formatDateTime(isoDate: string): string {
  const date = new Date(isoDate)
  
  const dateStr = new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(date)
  
  const timeStr = new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false
  }).format(date)
  
  return `${dateStr} - ${timeStr}`
}
