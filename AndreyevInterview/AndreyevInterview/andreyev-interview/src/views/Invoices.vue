<template>
  <div class="home">
    <form @submit.prevent>
      <label for="invoices">Create a new invoice</label>
      <input type="text" name="invoices" v-model="state.description" placeholder="Description" />
      <button @click="createInvoice">Create Invoice</button>
    </form>

    <hr />

    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Total Cost</th>
        <th>Total Value of Invoice</th>
        <th></th>
      </thead>
      <tbody>
        <tr v-for="invoice in state.invoices" :key="invoice.id">
          <td>{{invoice.id}}</td>
          <td>{{invoice.description}}</td>
          <td>${{invoice.totalValue}}</td>
          <td>${{invoice.totalBillable}}</td>
          <td>
            <router-link :to="{ 
              name: 'Invoice', 
              params: { id: invoice.id }
              }">
              Open
            </router-link>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive } from 'vue';
// @ is an alias to /src

export default defineComponent({
  name: 'Invoices',
  setup() {
    const state = reactive({
      invoices: [],
      description: "",
      totalValue: "0",
      totalBillable: "0",
    })

    function fetchInvoices() {
      fetch("http://localhost:5000/invoices", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(invoices => (state.invoices = invoices))
      })
    }

    function createInvoice() {
      fetch("http://localhost:5000/invoices", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          totalValue: Number(state.totalValue),
          totalBillable: Number(state.totalBillable)
        })
      }).then(fetchInvoices)
    }

    onMounted(fetchInvoices)

    return {state, createInvoice}
  }
});
</script>
