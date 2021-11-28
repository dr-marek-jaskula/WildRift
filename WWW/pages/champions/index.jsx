process.env.NODE_TLS_REJECT_UNAUTHORIZED="0";
import Image from "next/image";
import styles from "../../styles/ChampionPage.module.scss";
import { useState } from "react";
import Wrapper from "../../components/Wrapper";

const ChampionsPage = (props) => {

	const { darkmode, toggleDarkMode, championNames } = props;
	const fixName = (string) => {
		return string.replace(/[^a-zA-Z0-9]/g, "");
	};
	
	const fixedChamionNames = championNames.map((item) => ({...item, params: {name: fixName(item.params.name)}}));
	const [ championList, setChampionList ] = useState(fixedChamionNames);
	const [championTypes] = useState(["Assassin", "Defender", "Mage", "Marksman", "Support", "Warrior"]);

	return (
		<Wrapper darkmode={darkmode}>
			<main className={styles.championsPage}>
				<h1 className={styles.mainHeader}>Champions</h1>
				<div className={styles.filters}>
					<div className={styles.searchBarWrapper}>
						<label className={styles.championFilterLabel}>Search by name</label>
						<input type="text"/>
					</div>
					<div className={styles.championTypesFilters}>
						{championTypes.map(item => (
							<div key={item} className={styles.championTypeCheckbox}>
								<label className={styles.championFilterLabel}>{item}</label>
								<input type="checkbox" value={item}></input>
							</div>
						))}
					</div>
				</div>
				{championList.map((item, index) => {
					const { params } = item;
					const { name } = params;

			
					return <Image width="120" height="120" key={index} src={`/champions/thumbnails/${name}.png`}/>;}) }	
			</main>
		</Wrapper>
	);
};



export default ChampionsPage;

export async function getStaticProps() {
	const req = await fetch("https://localhost:5001/api/Champion/Names", {
		method: "GET",
		mode: "cors",
		cache: "default",
		headers: {
			"Content-Type":"application/json",
		}});
	const data = await req.json();
	const championNames = data.map(name => {
		return { params: {
			name
		}};
	});
	return {
		props: {
			championNames
		}
	};
}