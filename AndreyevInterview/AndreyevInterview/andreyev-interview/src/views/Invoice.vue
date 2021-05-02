<template>
  <div class="about">
    <router-link :to="{ name: 'Invoices' }">Back</router-link>

    <h2>Invoice Details</h2>

    <div>
      Invoice 
      <span style="font-weight:bold">#{{$route.params.id}}<br>
      Invoice Total Cost: ${{state.totalValue}}<br>
      Invoice Total Invoice: ${{state.totalBillable}}</span>
    </div>

    <h3>Line Items</h3>
    
    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Quantity</th>
        <th>Cost</th>
        <th>Billable
          <tr>/Non-Billable</tr>
        </th>
      </thead>
      <tbody>
        <tr v-for="item in state.lineItems" :key="item.id">
          <td>{{item.id}}</td>
          <td>{{item.description}}</td>
          <td>{{item.quantity}}</td>
          <td>{{item.cost}}</td>
          <td>
            <!-- <select class="enable" disabled=false>
              <option :selected=item.billable>Billable</option>
              <option :selected=item.billable>NonBillable</option>
            </select> -->
            <input type="checkbox" :checked=item.billable @change="changeBillable(item.id, item.billable)"/>
          </td>
        </tr>
      </tbody>
    </table>

    <form @submit.prevent>
      <h4>Create Line Item</h4>
      <input type="text" name="description" placeholder="Description" v-model="state.description" />
      <input type="number" name="quantity" min="1" placeholder="Quantity" v-model="state.quantity" />
      <input type="number" name="cost" step="any" min="0" placeholder="Cost" v-model="state.cost" />
      <button @click="createLineItem" v-bind:disabled="!state.description||!state.quantity||!state.cost">Create Invoice</button>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent, onMounted, reactive, ref} from "vue";


export default defineComponent({
  name: "Invoice",
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  setup(props) {
    const state = reactive({
      lineItems: [],
      description: "",
      quantity: "",
      cost: "",
      totalValue: "0",
      totalBillable: "0",
      billable: true
    })

    function fetchLineItems() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(
          lineItems => (state.lineItems = lineItems),
          )
      });

      fetch(`http://localhost:5000/invoices/${props.id}` + '/cost', {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(
          total => {
            state.totalValue = total[0],
            state.totalBillable = total[1]
          }
          )
      })
    }

    function createLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
          billable: state.billable
        })
      }).then(fetchLineItems)
    }

    function changeBillable(id:number, billable:boolean) {
      fetch(`http://localhost:5000/invoices/${props.id}` + '/' + id, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          invoiceId: props.id,
          id: id,
          billable: !billable
        })
      }).then(fetchLineItems);
    }

    onMounted(fetchLineItems)

    return {state, createLineItem, changeBillable}
  }

})
</script>