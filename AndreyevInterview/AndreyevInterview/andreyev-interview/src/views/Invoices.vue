<template>
  <div class="home">
    <form @submit.prevent>
      <label for="invoices">Create a new invoice</label>
      <input type="text" name="invoices" v-model="state.description" placeholder="Description"/>
      <div style="display:flex; justify-content:space-between">
        <!-- endpoint for JSON report-->
        <button @click="createInvoice" v-bind:disabled="!state.description">Create Invoice</button>
        <button onclick="window.location='https://localhost:5001/invoices'">View Report</button>
      </div>
    </form>

    <hr />

    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Total Cost</th>
        <th>Total Value of Invoice</th>
        <th>Number Of Items</th>
        <th></th>
      </thead>
      <tbody>
        <tr v-for="invoice in state.invoices" :key="invoice.id">
          <td>{{invoice.id}}</td>
          <td>{{invoice.description}}</td>
          <td>${{invoice.totalValue}}</td>
          <td>${{invoice.totalBillable}}</td>
          <td>{{invoice.numberOfItem}}</td>
          <td>
            <router-link :to="{ name: 'Invoice', params: { 
              id: invoice.id
              }}">
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
      numberOfItem: "0"
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
          totalBillable: Number(state.totalBillable),
          numberOfItem: Number(state.numberOfItem)
        })
      }).then(fetchInvoices)
    }

    onMounted(fetchInvoices)

    return {state, createInvoice}
  }
});
</script>
