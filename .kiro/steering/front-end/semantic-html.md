---
inclusion: manual
---

# Semantic HTML Standards

## HTML5 Semantic Elements

Always use semantic HTML5 elements for proper document structure:

### Document Structure

```vue
<template>
  <!-- Page layout with semantic elements -->
  <div id="app">
    <header class="site-header">
      <nav aria-label="Main navigation">
        <ul>
          <li><a href="/">Home</a></li>
          <li><a href="/projects">Projects</a></li>
          <li><a href="/transactions">Transactions</a></li>
        </ul>
      </nav>
    </header>

    <main id="main-content">
      <article class="project-detail">
        <header>
          <h1>{{ project.name }}</h1>
          <p class="meta">
            <time :datetime="project.createdAt">
              {{ formatDate(project.createdAt) }}
            </time>
          </p>
        </header>

        <section>
          <h2>Project Description</h2>
          <p>{{ project.description }}</p>
        </section>

        <section>
          <h2>Budget Information</h2>
          <dl>
            <dt>Total Budget</dt>
            <dd>{{ formatCurrency(project.budget) }}</dd>
            <dt>Current Balance</dt>
            <dd>{{ formatCurrency(project.balance) }}</dd>
          </dl>
        </section>

        <aside>
          <h3>Related Projects</h3>
          <ul>
            <li v-for="related in relatedProjects" :key="related.id">
              <a :href="`/projects/${related.id}`">{{ related.name }}</a>
            </li>
          </ul>
        </aside>
      </article>
    </main>

    <footer class="site-footer">
      <p>&copy; 2025 Project Budget Management</p>
    </footer>
  </div>
</template>
```
