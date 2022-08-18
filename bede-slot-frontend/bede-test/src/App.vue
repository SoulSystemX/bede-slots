<script>
import { defineComponent , onMounted, ref, computed, watch} from 'vue';


export default defineComponent({
 setup(){

  const gameboard = ref(null);
  const stake = ref(0)
  const balance = ref(0)
  const addToBalanceAmount = ref(0)
  const gameStarted = ref(false)
  const gameEnded = ref(false)
  const compBalance = computed(() => {return balance.value})

  onMounted(async () => {
    await GetBalance()
  });

 async function SpinWheel()
 {
    await fetch(`http://localhost:5230/Slot/Spin?stake=${stake.value}`)
    .then(response => response.json())
    .then(data => gameboard.value = data)

    await GetBalance()
    stake.value = 0
    
  }

  function ResetGame(){
    gameStarted.value = false;
    gameEnded.value = false;
    gameboard.value = null;
  }

  async function AddToBalance()
 {
    await fetch(`http://localhost:5230/Slot/AddToBalance?amountToAdd=${addToBalanceAmount.value}`, { method: "POST"})
    .then(response => response.json())
            .then(addToBalanceAmount.value = 0);

            GetBalance()
            gameStarted.value = true
  }

 async function GetBalance() 
 {
     await fetch(`http://localhost:5230/Slot/GetBalance`)
    .then(response => response.json())
     .then(data => balance.value = data.output.result);
  }

watch(balance, b => { if(b <= 0) { gameEnded.value = true}  });   


return{
  stake,
  SpinWheel,
  AddToBalance,
  gameboard,
  balance,
  addToBalanceAmount,
  GetBalance,
  compBalance,
  gameEnded,
  gameStarted,
  ResetGame

}

  }
});
// This starter template is using Vue 3 <script setup> SFCs
// Check out https://vuejs.org/api/sfc-script-setup.html#script-setup

</script>

<template>
  <div>

   
    <div >

<!-- {{balance}} -->
      <div class="mb-4">Balance : {{balance}}</div>
<div v-if="gameStarted === false"> 
  Would you like to add some money to your balance?

    <div>
      <div><input v-model="addToBalanceAmount" class="font-bold text-xs leading-tight uppercase w-64 h-8 rounded bg-white p-1 text-black" type="decimal" placeholder="Enter the amount to add to your balance" /></div>
      <div><button class="bg-blue-500 p-2 m-4 px-8 shadow font-bold text-xs leading-tight uppercase rounded shadow-md hover:bg-blue-700 hover:shadow-lg focus:bg-blue-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-blue-800 active:shadow-lg transition duration-150 ease-in-out" @click="AddToBalance">Add to balance</button></div>

    </div>
  </div>

    <div v-else class="m-4 border border-blue-500 p-8 shadow bg-gray-700">
      
    <div v-if="gameEnded === true">
      <div> Game Over!</div>
      <button class="bg-blue-500 p-2 m-4 px-8 shadow font-bold text-xs leading-tight uppercase rounded shadow-md hover:bg-blue-700 hover:shadow-lg focus:bg-blue-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-blue-800 active:shadow-lg transition duration-150 ease-in-out" @click="ResetGame">Start Again?</button>
    </div>
    <div v-else>
      <div>How much would you like to stake</div>
      <div><input v-model="stake" class="font-bold text-xs leading-tight uppercase w-64 h-8 rounded bg-white p-1 text-black" type="decimal" placeholder="Enter your Stake" /></div>
      <div>
        
        
        <button class="bg-blue-500 p-2 m-4 px-8 shadow font-bold text-xs leading-tight uppercase rounded shadow-md hover:bg-blue-700 hover:shadow-lg focus:bg-blue-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-blue-800 active:shadow-lg transition duration-150 ease-in-out" @click="SpinWheel">Spin</button></div>
    </div>
    </div>

    <div v-if="gameboard">
    <div class="flex " v-for="row in gameboard.output.gameboard">
      <div class="  m-auto my-2" v-for="cell in row">
        <div >
        <div >{{cell.symbol}}</div>
        </div>
        </div>
      <br/>
      </div>
    <div>
      <!-- New Balance : {{gameboard.output.balance}} -->
      Winnings : {{gameboard.output.winnings}}
  </div>
    
    </div> 

  </div>
  </div>

</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
