<script setup lang="ts">
import { AuditTrail, IntegrityVerification } from '@/components/audit'
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <!-- Page Header -->
    <header class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Audit & Data Integrity</h1>
      <p class="mt-2 text-sm text-gray-600">
        View system audit trail and verify data integrity to ensure accountability and detect
        tampering.
      </p>
    </header>

    <!-- Integrity Verification Section -->
    <section class="mb-12" aria-labelledby="integrity-heading">
      <h2 id="integrity-heading" class="mb-4 text-2xl font-semibold text-gray-900">
        Data Integrity Verification
      </h2>
      <IntegrityVerification />
    </section>

    <!-- Audit Trail Section -->
    <section aria-labelledby="audit-trail-heading">
      <h2 id="audit-trail-heading" class="mb-4 text-2xl font-semibold text-gray-900">
        Audit Trail
      </h2>
      <AuditTrail />
    </section>
  </div>
</template>
