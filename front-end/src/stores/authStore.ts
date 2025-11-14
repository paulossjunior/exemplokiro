import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

/**
 * Authentication store for managing user authentication state
 * Handles token storage, user information, and authentication status
 */
export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('auth_token'));
  const user = ref<{ id: string; name: string } | null>(null);

  // Computed
  const isAuthenticated = computed(() => !!token.value);

  // Actions
  /**
   * Set authentication token and persist to localStorage
   * @param newToken - JWT token from authentication
   */
  function setToken(newToken: string) {
    token.value = newToken;
    localStorage.setItem('auth_token', newToken);
  }

  /**
   * Set user information
   * @param userData - User data containing id and name
   */
  function setUser(userData: { id: string; name: string }) {
    user.value = userData;
  }

  /**
   * Logout user and cleanup authentication state
   * Removes token from localStorage and clears user data
   */
  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('auth_token');
  }

  return {
    // State
    token,
    user,
    // Computed
    isAuthenticated,
    // Actions
    setToken,
    setUser,
    logout,
  };
});
